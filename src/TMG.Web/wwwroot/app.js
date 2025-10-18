class TMGHeroApp {
    constructor() {
        this.connection = null;
        this.connectionStatus = {
            indicator: document.getElementById('connectionStatus'),
            text: document.getElementById('connectionText'),
            lastUpdate: document.getElementById('lastUpdate')
        };

        this.gameStatus = {
            bot: document.getElementById('botStatus'),
            game: document.getElementById('gameState')
        };

        this.controls = {
            init: document.getElementById('initBtn'),
            start: document.getElementById('startBtn'),
            stop: document.getElementById('stopBtn'),
            loadSave: document.getElementById('loadSaveBtn'),
            getSave: document.getElementById('getSaveBtn'),
            clearLog: document.getElementById('clearLogBtn'),
            openGame: document.getElementById('openGameBtn')
        };

        this.elements = {
            saveData: document.getElementById('saveData'),
            log: document.getElementById('log'),
            resourcesCard: document.getElementById('resourcesCard'),
            resourcesContainer: document.getElementById('resourcesContainer')
        };

        this.init();
    }

    async init() {
        this.setupEventHandlers();
        await this.initializeSignalR();
        await this.updateStatus();
    }

    setupEventHandlers() {
        this.controls.init.addEventListener('click', () => this.initializeGame());
        this.controls.start.addEventListener('click', () => this.startBot());
        this.controls.stop.addEventListener('click', () => this.stopBot());
        this.controls.loadSave.addEventListener('click', () => this.loadSave());
        this.controls.getSave.addEventListener('click', () => this.getCurrentSave());
        this.controls.clearLog.addEventListener('click', () => this.clearLog());
        this.controls.openGame.addEventListener('click', () => this.openGameInNewTab());
    }

    async initializeSignalR() {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("/gamehub")
            .withAutomaticReconnect()
            .build();

        this.connection.on("Connected", (message) => {
            this.log(`Connected: ${message}`, 'success');
            this.updateConnectionStatus(true);
        });

        this.connection.on("StatusChanged", (status) => {
            this.log(status, 'info');
            this.updateStatus();
        });

        this.connection.on("GameStateUpdated", (gameState) => {
            this.updateGameState(gameState);
        });

        this.connection.on("Error", (error) => {
            this.log(`Error: ${error}`, 'error');
        });

        this.connection.onreconnecting(() => {
            this.log("Connection lost. Reconnecting...", 'warning');
            this.updateConnectionStatus(false);
        });

        this.connection.onreconnected(() => {
            this.log("Reconnected successfully", 'success');
            this.updateConnectionStatus(true);
        });

        this.connection.onclose(() => {
            this.log("Connection closed", 'warning');
            this.updateConnectionStatus(false);
        });

        try {
            await this.connection.start();
            this.log("SignalR connected", 'success');
        } catch (err) {
            this.log(`SignalR connection failed: ${err}`, 'error');
            this.updateConnectionStatus(false);
        }
    }

    updateConnectionStatus(connected) {
        if (connected) {
            this.connectionStatus.indicator.className = 'status-indicator status-running';
            this.connectionStatus.text.textContent = 'Connected';
        } else {
            this.connectionStatus.indicator.className = 'status-indicator status-error';
            this.connectionStatus.text.textContent = 'Disconnected';
        }
        this.connectionStatus.lastUpdate.textContent = new Date().toLocaleTimeString();
    }

    async updateStatus() {
        try {
            const response = await fetch('/api/status');
            const status = await response.json();

            // Update bot status
            const botStatusSpan = this.gameStatus.bot.querySelector('span:last-child');
            const botIndicator = this.gameStatus.bot.querySelector('.status-indicator');

            if (status.isPlaying) {
                botStatusSpan.textContent = 'Running';
                botIndicator.className = 'status-indicator status-running';
                this.controls.start.disabled = true;
                this.controls.stop.disabled = false;
                this.controls.getSave.disabled = false;
            } else {
                botStatusSpan.textContent = 'Stopped';
                botIndicator.className = 'status-indicator status-stopped';
                this.controls.start.disabled = !status.hasPage;
                this.controls.stop.disabled = true;
                this.controls.getSave.disabled = !status.hasPage;
            }

            // Update game state
            const gameStateSpan = this.gameStatus.game.querySelector('span:last-child');
            const gameIndicator = this.gameStatus.game.querySelector('.status-indicator');

            if (status.hasPage) {
                gameStateSpan.textContent = 'Initialized';
                gameIndicator.className = 'status-indicator status-running';
                this.controls.init.disabled = false;
            } else {
                gameStateSpan.textContent = 'Not Initialized';
                gameIndicator.className = 'status-indicator status-stopped';
                this.controls.init.disabled = false;
            }

        } catch (error) {
            this.log(`Failed to update status: ${error.message}`, 'error');
        }
    }

    async initializeGame() {
        const saveData = this.elements.saveData.value.trim();
        this.controls.init.disabled = true;

        try {
            const response = await fetch('/api/initialize', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(saveData || null)
            });

            const result = await response.json();

            if (result.success) {
                this.log('Game initialized successfully', 'success');
                if (saveData) {
                    this.elements.saveData.value = '';
                }
            } else {
                this.log(`Initialization failed: ${result.message}`, 'error');
            }
        } catch (error) {
            this.log(`Initialization error: ${error.message}`, 'error');
        } finally {
            this.controls.init.disabled = false;
            await this.updateStatus();
        }
    }

    async startBot() {
        this.controls.start.disabled = true;

        try {
            const response = await fetch('/api/start', { method: 'POST' });
            const result = await response.json();

            if (result.success) {
                this.log('Bot started successfully', 'success');
            } else {
                this.log(`Failed to start bot: ${result.message}`, 'error');
            }
        } catch (error) {
            this.log(`Start error: ${error.message}`, 'error');
        } finally {
            await this.updateStatus();
        }
    }

    async stopBot() {
        this.controls.stop.disabled = true;

        try {
            const response = await fetch('/api/stop', { method: 'POST' });
            const result = await response.json();

            if (result.success) {
                this.log('Bot stopped successfully', 'success');
            } else {
                this.log(`Failed to stop bot: ${result.message}`, 'error');
            }
        } catch (error) {
            this.log(`Stop error: ${error.message}`, 'error');
        } finally {
            await this.updateStatus();
        }
    }

    async loadSave() {
        const saveData = this.elements.saveData.value.trim();

        if (!saveData) {
            this.log('Please paste save data first', 'warning');
            return;
        }

        await this.initializeGame();
    }

    async getCurrentSave() {
        this.controls.getSave.disabled = true;

        try {
            const response = await fetch('/api/save');
            const result = await response.json();

            if (result.success) {
                this.elements.saveData.value = result.saveData;
                this.log('Save data retrieved successfully', 'success');
            } else {
                this.log(`Failed to get save data: ${result.message}`, 'error');
            }
        } catch (error) {
            this.log(`Get save error: ${error.message}`, 'error');
        } finally {
            this.controls.getSave.disabled = false;
        }
    }

    clearLog() {
        this.elements.log.innerHTML = '<div class="text-muted">Log cleared</div>';
    }

    openGameInNewTab() {
        window.open('https://www.theresmoregame.com/play', '_blank');
    }

    updateGameState(gameState) {
        // Show resources card
        this.elements.resourcesCard.style.display = 'block';

        // Update resources
        let resourcesHtml = '';

        Object.entries(gameState.resources).forEach(([name, resource]) => {
            const percentage = resource.cap > 0 ? (resource.amount / resource.cap * 100) : 0;
            const incomeText = resource.income !== 0 ? ` (${resource.income > 0 ? '+' : ''}${resource.income}/s)` : '';

            resourcesHtml += `
                <div class="resource-card card mb-2">
                    <div class="card-body py-2">
                        <div class="d-flex justify-content-between align-items-center mb-1">
                            <span class="fw-semibold">${this.capitalizeFirst(name)}</span>
                            <small class="text-muted">${this.formatNumber(resource.amount)}${resource.cap > 0 ? `/${this.formatNumber(resource.cap)}` : ''}${incomeText}</small>
                        </div>
                        ${resource.cap > 0 ? `
                            <div class="progress progress-bar-custom">
                                <div class="progress-bar ${this.getProgressBarColor(percentage)}"
                                     style="width: ${percentage}%"></div>
                            </div>
                        ` : ''}
                    </div>
                </div>
            `;
        });

        this.elements.resourcesContainer.innerHTML = resourcesHtml;

        // Log the update
        this.log(`Game state updated: ${gameState.buildings} buildings, ${Object.keys(gameState.resources).length} resources`, 'info');
    }

    capitalizeFirst(str) {
        return str.charAt(0).toUpperCase() + str.slice(1);
    }

    formatNumber(num) {
        if (num >= 1000000) {
            return (num / 1000000).toFixed(1) + 'M';
        } else if (num >= 1000) {
            return (num / 1000).toFixed(1) + 'K';
        }
        return Math.floor(num).toString();
    }

    getProgressBarColor(percentage) {
        if (percentage >= 90) return 'bg-danger';
        if (percentage >= 70) return 'bg-warning';
        return 'bg-primary';
    }

    log(message, type = 'info') {
        const timestamp = new Date().toLocaleTimeString();
        const typeClasses = {
            'success': 'text-success',
            'error': 'text-danger',
            'warning': 'text-warning',
            'info': 'text-info'
        };

        const logEntry = document.createElement('div');
        logEntry.className = 'fade-in';
        logEntry.innerHTML = `
            <span class="text-muted">[${timestamp}]</span>
            <span class="${typeClasses[type] || 'text-muted'}">${message}</span>
        `;

        this.elements.log.appendChild(logEntry);
        this.elements.log.scrollTop = this.elements.log.scrollHeight;

        // Keep only last 100 log entries
        while (this.elements.log.children.length > 100) {
            this.elements.log.removeChild(this.elements.log.firstChild);
        }
    }
}

// Initialize the app when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    new TMGHeroApp();
});
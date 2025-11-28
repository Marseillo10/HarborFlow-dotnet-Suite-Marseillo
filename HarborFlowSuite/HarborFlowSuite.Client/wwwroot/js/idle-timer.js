window.idleTimer = {
    timer: null,
    dotNetHelper: null,
    timeoutInMs: 900000, // Default 15 minutes

    initialize: function (dotNetHelper, timeoutInMs) {
        this.dotNetHelper = dotNetHelper;
        this.timeoutInMs = timeoutInMs;
        this.resetTimer();

        // Events to track activity
        window.addEventListener('mousemove', this.resetTimer.bind(this));
        window.addEventListener('mousedown', this.resetTimer.bind(this));
        window.addEventListener('keypress', this.resetTimer.bind(this));
        window.addEventListener('touchmove', this.resetTimer.bind(this));
        window.addEventListener('scroll', this.resetTimer.bind(this));
    },

    resetTimer: function () {
        if (this.timer) {
            clearTimeout(this.timer);
        }

        // Debounce slightly to avoid excessive calls if we were doing more complex logic,
        // but for simple clearTimeout/setTimeout it's fine.
        this.timer = setTimeout(this.logout.bind(this), this.timeoutInMs);
    },

    logout: function () {
        if (this.dotNetHelper) {
            this.dotNetHelper.invokeMethodAsync('Logout');
        }
    },

    dispose: function () {
        if (this.timer) {
            clearTimeout(this.timer);
        }
        window.removeEventListener('mousemove', this.resetTimer.bind(this));
        window.removeEventListener('mousedown', this.resetTimer.bind(this));
        window.removeEventListener('keypress', this.resetTimer.bind(this));
        window.removeEventListener('touchmove', this.resetTimer.bind(this));
        window.removeEventListener('scroll', this.resetTimer.bind(this));
    }
};

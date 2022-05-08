window.Util = (() => {
    return {
        getWindowDimensions() {
            return {
                width: window.innerWidth,
                heigh: window.innerHeight
            }
        },

        getBoundingBox(elementRef) {
            var element = document.querySelector(elementRef);
            if (element) {
                return element.getBoundingClientRect();
            }

            return null;
        }
    };
})();
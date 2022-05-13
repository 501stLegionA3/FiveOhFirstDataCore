window.Util = (() => {
    let keyDownHandlers = {};

    return {
        getWindowDimensions() {
            return {
                width: window.innerWidth,
                height: window.innerHeight
            }
        },

        getBoundingBox(elementRef) {
            var element = document.querySelector(`[data-id="${elementRef}"]`);
            if (element) {
                return element.getBoundingClientRect();
            }

            return null;
        },

        registerKeyDownHandler(dotNetRef, method) {
            let controller = new AbortController();

            options = {
                signal: controller.signal
            };

            document.addEventListener('keydown', async (x) => {
                if (x.defaultPrevented || x.isComposing) {
                    return;
                }

                if (!(dotNetRef in keyDownHandlers)) {
                    controller.abort('Removed by client.');
                    return;
                }

                await dotNetRef.invokeMethodAsync(method, {
                    Key: x.key,
                    ShiftKey: x.shiftKey,
                    CtrlKey: x.ctrlKey,
                    AltKey: x.altKey,
                    MetaKey: x.metaKey
                });

                x.preventDefault();
            }, options)

            keyDownHandlers[dotNetRef] = controller;
        },

        removeKeyDownHandler(dotNetRef) {
            if (dotNetRef in keyDownHandlers) {
                keyDownHandlers[dotNetRef].abort('Removed by server.');
                delete keyDownHandler[dotNetRef];
            }
        }
    };
})();
import Split from 'split-grid'

window.SplitInterop = (() => {
    const liveNodes = {};

    return {
        createSplit(guid, dotNetRef, sizeUpdateMethod, rows = {}, cols = {}) {
            let columnGutters = [];
            let rowGutters = [];

            let head = document.querySelector("#" + guid);

            if (head) {
                for (item in rows) {
                    rowGutters.push({
                        track: item,
                        element: head.querySelector(rows[item])
                    });
                }

                for (item in cols) {
                    columnGutters.push({
                        track: item,
                        element: head.querySelector(rows[item])
                    });
                }

                conf = {
                    columnGutters,
                    rowGutters,
                    writeStyle: (grid, gridTemplateProp, gridTemplateStlye) => {
                        dotNetRef.invokeMethodAsync(sizeUpdateMethod, gridTemplateStlye);
                        grid.style[gridTemplateProp] = gridTemplateStyle
                    }
                }

                let inst = Split(conf);
                liveNodes[guid] = inst;
            }
        },

        destroy(guid) {
            let inst = liveNodes[guid];
            if (inst) {
                inst.destroy();
                liveNodes.delete(guid);
            }
        },

        insertRow(guid, index, selector) {
            let element = document.querySelector(selector);
            let split = liveNodes[guid];
            split.addRowGutter(element, index);
        },

        insertCol(guid, index, selector) {
            let element = document.querySelector(selector);
            let split = liveNodes[guid];
            split.addColumnGutter(element, index);
        },

        removeRow(guid, index) {
            let element = document.querySelector(selector);
            let split = liveNodes[guid];
            split.addColumnGutter(element, index);
        },

        removeCol(guid, index) {
            let element = document.querySelector(selector);
            let split = liveNodes[guid];
            split.addRowGutter(element, index);
        }
    }
})();
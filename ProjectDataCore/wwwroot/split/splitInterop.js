import Split from 'split-grid'

window.SplitInterop = (() => {
    const liveNodes = {};

    return {
        createSplit(guid, dotNetRef, sizeUpdateMethod, rows = {}, cols = {}) {
            try {

                let columnGutters = [];
                let rowGutters = [];

                let head = document.querySelector(`[data-split-container="${guid}"]`);

                if (head) {
                    for (var item in rows) {
                        if (rows[item] !== "") {
                            rowGutters.push({
                                track: parseInt(item),
                                element: head.querySelector(rows[item])
                            });
                        }
                    }

                    for (var item in cols) {
                        if (cols[item] !== "") {
                            columnGutters.push({
                                track: parseInt(item),
                                element: head.querySelector(cols[item])
                            });
                        }
                    }

                    let conf = {
                        columnGutters,
                        rowGutters,
                        writeStyle: (grid, gridTemplateProp, gridTemplateStyle) => {
                            dotNetRef.invokeMethodAsync(sizeUpdateMethod, gridTemplateStyle);
                            grid.style[gridTemplateProp] = gridTemplateStyle;
                        },
                        minSize: 40,
                    }

                    let inst = Split(conf);
                    liveNodes[guid] = inst;
                }

            } catch (err) {
                console.error(err);
            }
        },

        destroy(guid) {
            let inst = liveNodes[guid];
            if (inst) {
                inst.destroy();
                delete liveNodes[guid];
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
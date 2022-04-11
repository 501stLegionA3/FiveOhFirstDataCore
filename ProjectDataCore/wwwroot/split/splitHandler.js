import Split from 'split-grid'

const liveNodes = {};

export function createSplit(guid, dotNetRef, sizeUpdateMethod, rows = {}, cols = {}) {
    let columnGutters = [];
    let rowGutters = [];

    for (item in rows) {
        rowGutters.push({
            track: item,
            element: document.querySelector(rows[item])
        });
    }

    for (item in cols) {
        columnGutters.push({
            track: item,
            element: document.querySelector(rows[item])
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

export function insertRow(guid, index, selector) {
    let element = document.querySelector(selector);
    let split = liveNodes[guid];
    split.addRowGutter(element, index);
}

export function insertCol(guid, index, selector) {
    let element = document.querySelector(selector);
    let split = liveNodes[guid];
    split.addColumnGutter(element, index);
}

export function removeRow(guid, index) {
    let element = document.querySelector(selector);
    let split = liveNodes[guid];
    split.addColumnGutter(element, index);
}

export function removeCol(guid, index) {
    let element = document.querySelector(selector);
    let split = liveNodes[guid];
    split.addRowGutter(element, index);
}
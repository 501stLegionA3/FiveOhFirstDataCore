import { Droppable } from '@shopify/draggable';

window.DropInterop = (() => {
    const liveDrops = {};
    const dropzones = {};

    return {
        init(guid, dotNetRef, uniqueDrops = false, dragChangeMethod = "", draggable = '.drag-item', dropzone = '.dropzone') {
            const elements = document.querySelectorAll(`[data-drag-container="${guid}"]`);

            const drop = new Droppable(elements, {
                draggable: draggable,
                dropzone: dropzone,
                mirror: {
                    constrainDimensions: true,
                },
            });

            let itemType;
            let dropZone;

            drop.on('drag:start', (evt) => {
                itemType = evt.source.dataset.itemType;

                if (uniqueDrops) {
                    dropZone = evt.source.dataset.dropZone;
                }

                dotNetRef.invokeMethodAsync(dragChangeMethod, true);
            });

            drop.on('drag:stop', () => {
                dotNetRef.invokeMethodAsync(dragChangeMethod, false);
            });

            drop.on('droppable:stop', (evt) => {
                if (uniqueDrops) {
                    if (dropZone !== evt.dropzone.dataset.dropZone) {
                        evt.cancel();
                        return;
                    }
                }

                let dropZoneId = evt.dropzone.dataset.refId;
                let droppedOn = dropzones[dropZoneId];

                if (droppedOn) {
                    let destType = evt.dropzone.dataset.itemType;
                    droppedOn.ref.invokeMethodAsync(droppedOn.method, itemType, destType);
                }

                dotNetRef.invokeMethodAsync(dragChangeMethod, false);

                // No matter what, we cancel this operation
                evt.cancel();
            });

            liveDrops[guid] = drop;
        },

        destroyDroppable(guid) {
            try {
                let item = liveDrops[guid];
                if (item) {
                    item.destroy();
                    delete liveDrops[guid];
                }
            } catch (err) {
                console.log(err);
            }
        },

        registerDropzone(guid, dotNetRef, dropMethod) {
            dropzones[guid] = {
                ref: dotNetRef,
                method: dropMethod
            };
        },

        destroyDropzone(guid) {
            let item = dropzones[guid];
            if (item) {
                delete dropzones[guid];
            }
        }
    }
})();
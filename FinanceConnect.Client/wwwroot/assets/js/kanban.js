window.kanbanSortables = [];

window.initKanban = function (dotnetHelper) {

    if (window.kanbanSortables.length > 0) {
        window.kanbanSortables.forEach(s => s.destroy());
        window.kanbanSortables = [];
    }

    const columns = document.querySelectorAll(".kanban-body");

    columns.forEach(column => {

        const sortable = new Sortable(column, {

            group: "kanban",

            animation: 350,

            easing: "cubic-bezier(.2,1,.3,1)",

            direction: "vertical",   // IMPORTANT

            ghostClass: "kanban-ghost",

            chosenClass: "kanban-chosen",

            dragClass: "kanban-drag",

            swapThreshold: 0.3,      // allow easy vertical swap

            invertSwap: true,        // improves vertical movement

            onStart: function () {
                document.body.classList.add("kanban-dragging");
            },

            onEnd: function (evt) {

                document.body.classList.remove("kanban-dragging");

                const taskId = evt.item.dataset.taskid;
                const newStatusId = evt.to.dataset.statusid;
                const oldStatusId = evt.from.dataset.statusid;

                // Only revert if moved to another column
                if (newStatusId !== oldStatusId) {
                    evt.from.appendChild(evt.item);
                }

                dotnetHelper.invokeMethodAsync(
                    "UpdateTaskStatus",
                    taskId,
                    newStatusId
                );
            }
        });

        window.kanbanSortables.push(sortable);
    });
};
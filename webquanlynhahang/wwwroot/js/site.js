document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('form[data-confirm]').forEach(form => {
        form.addEventListener('submit', event => {
            const message = form.dataset.confirm || 'Bạn chắc chắn muốn thực hiện thao tác này?';
            if (!window.confirm(message)) event.preventDefault();
        });
    });
    document.querySelectorAll('button[data-confirm]').forEach(button => {
        button.addEventListener('click', event => {
            const message = button.dataset.confirm || 'Bạn chắc chắn muốn thực hiện thao tác này?';
            if (!window.confirm(message)) event.preventDefault();
        });
    });
});

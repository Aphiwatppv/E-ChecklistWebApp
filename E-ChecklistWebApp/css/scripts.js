document.addEventListener('DOMContentLoaded', function () {
    const sections = document.querySelectorAll('.fade-in-section');

    const observer = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('is-visible');
            } else {
                entry.target.classList.remove('is-visible');
            }
        });
    }, { threshold: 0.1 });

    sections.forEach(section => {
        observer.observe(section);
    });


    const searchBar = document.getElementById('searchBar');
    if (searchBar) {
        searchBar.addEventListener('keyup', function () {
            var searchQuery = this.value.toLowerCase();
            var cards = document.getElementsByClassName('checklist-card');

            Array.prototype.forEach.call(cards, function (card) {
                var cardTitle = card.getAttribute('data-title').toLowerCase();
                if (searchQuery === "" || cardTitle.includes(searchQuery)) {
                    card.style.display = '';
                } else {
                    card.style.display = 'none';
                }
            });
        });
    }

});

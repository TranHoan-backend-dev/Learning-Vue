export const handleSearching = () => {
    const searchField = document.querySelector(".navbar__left__search__input");
    searchField.addEventListener('keydown', function (e) {
        if (e.key === 'Enter') {
            const keyword = this.value.trim();
        }
    })
}
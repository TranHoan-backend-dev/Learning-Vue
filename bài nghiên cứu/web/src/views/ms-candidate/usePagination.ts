import {ref, computed, type Ref} from 'vue';

/**
 * Composable for handling pagination logic.
 * @param data Ref containing the full array of data to be paginated.
 */
export function usePagination<T>(data: Ref<T[]>) {
    const currentPage = ref(1);
    const pageSize = ref(10);

    const totalRecords = computed(() => data.value.length);
    const totalPages = computed(() => Math.ceil(totalRecords.value / pageSize.value));

    const paginatedData = computed(() => {
        const start = (currentPage.value - 1) * pageSize.value;
        const end = start + pageSize.value;
        return data.value.slice(start, end);
    });

    const pageInfo = computed(() => {
        const start = totalRecords.value > 0 ? (currentPage.value - 1) * pageSize.value + 1 : 0;
        const end = Math.min(currentPage.value * pageSize.value, totalRecords.value);
        return `${start} - ${end} / ${totalRecords.value} bản ghi`;
    });

    const handlePageSizeChange = () => {
        currentPage.value = 1;
    };

    const handleNextPage = () => {
        if (currentPage.value < totalPages.value) {
            currentPage.value++;
        }
    };

    const handlePrevPage = () => {
        if (currentPage.value > 1) {
            currentPage.value--;
        }
    };

    return {
        currentPage,
        pageSize,
        totalRecords,
        totalPages,
        paginatedData,
        pageInfo,
        handlePageSizeChange,
        handleNextPage,
        handlePrevPage,
    };
}

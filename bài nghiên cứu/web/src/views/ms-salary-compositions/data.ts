export type SalaryCompositions = {
    componentId: string,
    componentName: string,
    appliedFor: string,
    componentType: string,
    attribute: 'Khác' | 'Thu nhập' | 'Khấu trừ',
    valueType: 'Số' | 'Tiền tệ' | 'Chữ' | 'Ngày',
    value: string,
    source: string,
    status: number
}
export const pageSizeOptions = [
    { value: 15, label: "15" },
    { value: 25, label: "25" },
    { value: 50, label: "50" },
    { value: 100, label: "100" },
];

export type DataTableAttributes = {
    tableData: any[];
    totalRecords: number;
    pageSize: number;
    currentPage: number;
    columns: any[];
    pageInfo: string;
    selectedIds: string[];
    searchKeyword: string;
    isSystemMode?: boolean;
    systemItems?: any[];
    selectedSystemId?: string;
    statusFilter: any;
};

export const salaryCompositionStatus = [
    { text: 'Tất cả trạng thái', value: 'all' },
    { text: 'Đang theo dõi', value: 1 },
    { text: 'Ngừng theo dõi', value: 0 }
];

export const attributeOptions = [
    { id: 0, name: 'Khác' },
    { id: 1, name: 'Thu nhập' },
    { id: 2, name: 'Khấu trừ' }
];
export const valueTypeOptions = [
    { id: 0, name: 'Số' },
    { id: 1, name: 'Tiền tệ' },
    { id: 2, name: 'Chữ' },
    { id: 3, name: 'Ngày' },
    { id: 4, name: 'Phần trăm' }
];

export const getAttributeName = (id: number) => {
    return attributeOptions.find(opt => opt.id === id)?.name || 'Khác';
};

export const getValueTypeName = (id: number) => {
    return valueTypeOptions.find(opt => opt.id === id)?.name || '-';
};
import {ref} from "vue";

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

export const gridActions = [
    { id: 'active', icon: 'check-circle' as const, color: '#2ca01c', title: 'Sử dụng', class: 'action-active' },
    { id: 'copy', icon: 'copy' as const, color: '#5a5a5a', title: 'Nhân bản', class: 'action-copy' },
    { id: 'edit', icon: 'edit' as const, color: '#5a5a5a', title: 'Sửa', class: 'action-edit' },
    { id: 'delete', icon: 'trash' as const, color: '#ff4d4f', title: 'Xóa', class: 'action-delete' },
];

export const pageSizeOptions = [
    { value: 5, label: "5" },
    { value: 10, label: "10" },
    { value: 15, label: "15" },
    { value: 25, label: "25" },
    { value: 50, label: "50" },
];

export const systemDirectoryColumns = ref([
    { dataField: 'componentCode', caption: 'Mã thành phần', visible: true, width: 250, isPinned: true },
    { dataField: 'componentName', caption: 'Tên thành phần', visible: true, width: 250, isPinned: true },
    { dataField: 'salaryComponentSystemName', caption: 'Loại thành phần', visible: true, width: 150, isPinned: false },
    { dataField: 'attribute', caption: 'Tính chất', visible: true, width: 120, isPinned: false },
    { dataField: 'valueType', caption: 'Kiểu giá trị', visible: true, width: 120, isPinned: false },
    { dataField: 'value', caption: 'Giá trị', visible: true, width: 200, isPinned: false },
]);
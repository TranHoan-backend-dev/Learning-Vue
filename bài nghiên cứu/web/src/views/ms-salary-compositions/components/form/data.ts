export const defaultData = {
    componentName: '',
    componentId: '',
    appliedUnitId: null as string | null,
    salaryComponentSystemId: null as string | null,
    attribute: 1, // Default: Thu nhập (1)
    taxType: null as string | null,
    quota: '',
    allowExceedQuota: false,
    valueType: 1, // Default: Tiền tệ (1)
    valueCalculation: 'Tính theo công thức tự đặt',
    valueCalculationTarget: 'Trong cùng đơn vị công tác',
    valueFormula: '',
    description: '',
    showOnPayslip: 'Có',
    sourceType: 'Tự thêm'
}

export const attributeOptions = [
    { id: 1, name: 'Thu nhập' },
    { id: 2, name: 'Khấu trừ' }
];
export const valueTypeOptions = [
    { id: 1, name: 'Tiền tệ' },
    { id: 2, name: 'Phần trăm' },
    { id: 3, name: 'Hệ số' }
];
export const calculationTargetOptions = ['Trong cùng đơn vị công tác', 'Toàn công ty'];
export const showOnPayslipOptions = [
    { value: 'Có', label: 'Có', tabindex: 16 },
    { value: 'Không', label: 'Không', tabindex: 17 },
    { value: 'Chỉ hiển thị nếu giá trị khác 0', label: 'Chỉ hiển thị nếu giá trị khác 0', tabindex: 18 }
];

export const taxOptions = [
    { value: 'Chịu thuế', label: 'Chịu thuế', tabindex: 6 },
    { value: 'Miễn thuế toàn phần', label: 'Miễn thuế toàn phần', tabindex: 7 },
    { value: 'Miễn thuế một phần', label: 'Miễn thuế một phần', tabindex: 8 }
];
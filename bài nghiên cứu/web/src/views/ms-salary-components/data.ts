export type SalaryComponents = {
    componentId: string,
    componentName: string,
    appliedFor: string,
    componentType: string,
    attribute: 'Khác' | 'Thu nhập' | 'Khấu trừ',
    valueType: 'Số' | 'Tiền tệ' | 'Chữ' | 'Ngày',
    value: string
}

export const salaryComponentsData: SalaryComponents[] = [
    {
        componentId: '1',
        componentName: '1',
        appliedFor: 'CÔNG TY CP INTEL',
        componentType: 'Thông tin nhân viên',
        attribute: 'Khác',
        valueType: 'Số',
        value: '-'
    },
    {
        componentId: '465ERTERT',
        componentName: 'PC CKH121',
        appliedFor: 'CÔNG TY CP INTEL',
        componentType: 'Doanh số',
        attribute: 'Thu nhập',
        valueType: 'Tiền tệ',
        value: '1000000'
    },
    {
        componentId: 'A',
        componentName: 'A',
        appliedFor: 'CÔNG TY CP INTEL',
        componentType: 'Khác',
        attribute: 'Thu nhập',
        valueType: 'Tiền tệ',
        value: '300'
    },
    {
        componentId: 'AVA',
        componentName: 'ava',
        appliedFor: 'CÔNG TY CP INTEL',
        componentType: 'Thông tin nhân viên',
        attribute: 'Thu nhập',
        valueType: 'Tiền tệ',
        value: '= TONG_CONG_...'
    },
    {
        componentId: 'BAC_LUONG',
        componentName: 'Bậc lương',
        appliedFor: 'CÔNG TY CP INTEL',
        componentType: 'Thông tin nhân viên',
        attribute: 'Khác',
        valueType: 'Chữ',
        value: '-'
    },
    {
        componentId: 'BANG_THUE',
        componentName: 'Bảng thuế',
        appliedFor: 'CÔNG TY CP INTEL',
        componentType: 'Lương',
        attribute: 'Khác',
        valueType: 'Ngày',
        value: '5.000.000'
    },
    {
        componentId: 'BHTN',
        componentName: 'BHTN',
        appliedFor: 'CÔNG TY CP INTEL',
        componentType: 'Bảo hiểm - Công đoàn',
        attribute: 'Khấu trừ',
        valueType: 'Tiền tệ',
        value: '=BHTN'
    },
    {
        componentId: 'BHTN_CONG_TY_DONG',
        componentName: 'BHTN (Công ty đóng)',
        appliedFor: 'CÔNG TY CP INTEL',
        componentType: 'Bảo hiểm - Công đoàn',
        attribute: 'Khác',
        valueType: 'Tiền tệ',
        value: '=BHTN_CONG_T...'
    },
    {
        componentId: 'BHXH',
        componentName: 'BHXH',
        appliedFor: 'CÔNG TY CP INTEL',
        componentType: 'Bảo hiểm - Công đoàn',
        attribute: 'Khấu trừ',
        valueType: 'Tiền tệ',
        value: '=ROUND(BHXH, 1)'
    },
    {
        componentId: 'BHYT',
        componentName: 'BHYT',
        appliedFor: 'CÔNG TY CP INTEL',
        componentType: 'Bảo hiểm - Công đoàn',
        attribute: 'Khấu trừ',
        valueType: 'Tiền tệ',
        value: '=BHYT'
    },
    {
        componentId: 'CA_DAC_BIET',
        componentName: 'Ca đặc biệt',
        appliedFor: 'CÔNG TY CP INTEL',
        componentType: 'Chấm công',
        attribute: 'Khác',
        valueType: 'Số',
        value: '-'
    },
    {
        componentId: 'CDP',
        componentName: 'Công đoàn phí',
        appliedFor: 'CÔNG TY CP INTEL',
        componentType: 'Bảo hiểm - Công đoàn',
        attribute: 'Khấu trừ',
        valueType: 'Tiền tệ',
        value: '=if( TINH_CHAT_L... )'
    },
    {
        componentId: 'CHECKBOX',
        componentName: 'Checkbox',
        appliedFor: 'CÔNG TY CP INTEL',
        componentType: 'Thông tin nhân viên',
        attribute: 'Khác',
        valueType: 'Chữ',
        value: '-'
    },
    {
        componentId: 'CHUYEN_DOANH_SO',
        componentName: 'Chuyên doanh số',
        appliedFor: 'CÔNG TY CP INTEL',
        componentType: 'Lương',
        attribute: 'Thu nhập',
        valueType: 'Tiền tệ',
        value: '=E6/2'
    },
    {
        componentId: 'COMBOBOX',
        componentName: 'combobox',
        appliedFor: 'CÔNG TY CP INTEL',
        componentType: 'Thông tin nhân viên',
        attribute: 'Khác',
        valueType: 'Chữ',
        value: '-'
    },
    {
        componentId: 'CONG_AN_CA',
        componentName: 'Công ăn ca',
        appliedFor: 'CÔNG TY CP INTEL',
        componentType: 'Chấm công',
        attribute: 'Khác',
        valueType: 'Số',
        value: '-'
    },
    {
        componentId: 'PC_AN_TRUA',
        componentName: 'Phụ cấp ăn trưa',
        appliedFor: 'CÔNG TY CP INTEL',
        componentType: 'Lương',
        attribute: 'Thu nhập',
        valueType: 'Tiền tệ',
        value: '730000'
    },
    {
        componentId: 'PC_DIEN_THOAI',
        componentName: 'Phụ cấp điện thoại',
        appliedFor: 'CÔNG TY CP INTEL',
        componentType: 'Lương',
        attribute: 'Thu nhập',
        valueType: 'Tiền tệ',
        value: '200000'
    },
    {
        componentId: 'THU_NHAP_KHAC',
        componentName: 'Thu nhập khác',
        appliedFor: 'CÔNG TY CP INTEL',
        componentType: 'Khác',
        attribute: 'Thu nhập',
        valueType: 'Tiền tệ',
        value: '0'
    }
];
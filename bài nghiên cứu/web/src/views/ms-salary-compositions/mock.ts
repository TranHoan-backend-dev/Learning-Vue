/**
 * Mock data for Salary Compositions and System Directory
 * Used when backend is unavailable
 */

export const mockSalaryCompositions = [
    {
        salaryComponentId: '1',
        salaryComponentCode: 'LCB',
        salaryComponentName: 'Lương cơ bản',
        appliedUnitId: 'org-1',
        appliedUnitName: 'Công ty Cổ phần MISA',
        salaryComponentSystemId: 'salary',
        salaryComponentSystemName: 'Lương',
        attribute: 1, // Thu nhập
        valueType: 1,  // Tiền tệ
        value: '5,000,000',
        status: 1,    // Đang sử dụng
        source: 'Tự thêm'
    },
    {
        salaryComponentId: '2',
        salaryComponentCode: 'PCAT',
        salaryComponentName: 'Phụ cấp ăn trưa',
        appliedUnitId: 'org-1',
        appliedUnitName: 'Công ty Cổ phần MISA',
        salaryComponentSystemId: 'salary',
        salaryComponentSystemName: 'Lương',
        attribute: 1, // Thu nhập
        valueType: 1,  // Tiền tệ
        value: '730,000',
        status: 1,
        source: 'Tự thêm'
    },
    {
        salaryComponentId: '3',
        salaryComponentCode: 'BHXH',
        salaryComponentName: 'Bảo hiểm xã hội (8%)',
        appliedUnitId: 'org-1',
        appliedUnitName: 'Công ty Cổ phần MISA',
        salaryComponentSystemId: 'insurance',
        salaryComponentSystemName: 'Bảo hiểm',
        attribute: 2, // Khấu trừ
        valueType: 1,  // Tiền tệ
        value: '=(LCB + PCAT) * 0.08',
        status: 1,
        source: 'Hệ thống'
    },
    {
        salaryComponentId: '4',
        salaryComponentCode: 'THUONG_KPI',
        salaryComponentName: 'Thưởng KPI',
        appliedUnitId: 'org-1',
        appliedUnitName: 'Công ty Cổ phần MISA',
        salaryComponentSystemId: 'kpi',
        salaryComponentSystemName: 'KPI',
        attribute: 1,
        valueType: 1,
        value: '1,000,000',
        status: 1,
        source: 'Tự thêm'
    },
    {
        salaryComponentId: '5',
        salaryComponentCode: 'LUONG_OT',
        salaryComponentName: 'Lương làm thêm giờ',
        appliedUnitId: 'org-1',
        appliedUnitName: 'Công ty Cổ phần MISA',
        salaryComponentSystemId: 'attendance',
        salaryComponentSystemName: 'Chấm công',
        attribute: 1,
        valueType: 1,
        value: '-',
        status: 0, // Ngừng sử dụng
        source: 'Hệ thống'
    }
];

export const mockSystemCompositions = [
    {
        salaryComponentId: 'sys-1',
        salaryComponentCode: 'TY_LE_HOAN_THANH_KPI',
        salaryComponentName: 'Tỷ lệ hoàn thành KPI',
        salaryComponentSystemId: 'kpi',
        salaryComponentSystemName: 'KPI',
        attribute: 0,
        valueType: 0,
        value: '-',
        source: 'Hệ thống'
    },
    {
        salaryComponentId: 'sys-2',
        salaryComponentCode: 'TY_LE_HOAN_THANH_DOANH_SO',
        salaryComponentName: 'Tỷ lệ hoàn thành doanh số',
        salaryComponentSystemId: 'sales',
        salaryComponentSystemName: 'Doanh số',
        attribute: 0,
        valueType: 0,
        value: '=DOANH_SO_THUC_T...',
        source: 'Hệ thống'
    },
    {
        salaryComponentId: 'sys-3',
        salaryComponentCode: 'TONG_GIO_LAM_THEM_HUONG_LUONG_THU_VIEC',
        salaryComponentName: 'Tổng giờ làm thêm hưởng lương thử việc',
        salaryComponentSystemId: 'attendance',
        salaryComponentSystemName: 'Chấm công',
        attribute: 0,
        valueType: 0,
        value: '-',
        source: 'Hệ thống'
    },
    {
        salaryComponentId: 'sys-4',
        salaryComponentCode: 'TONG_GIO_LAM_THEM_HUONG_LUONG_KHAC',
        salaryComponentName: 'Tổng giờ làm thêm hưởng lương khác',
        salaryComponentSystemId: 'attendance',
        salaryComponentSystemName: 'Chấm công',
        attribute: 0,
        valueType: 0,
        value: '-',
        source: 'Hệ thống'
    },
    {
        salaryComponentId: 'sys-5',
        salaryComponentCode: 'TONG_GIO_LAM_THEM_HUONG_LUONG_HOC_VIEC',
        salaryComponentName: 'Tổng giờ làm thêm hưởng lương học việc',
        salaryComponentSystemId: 'attendance',
        salaryComponentSystemName: 'Chấm công',
        attribute: 0,
        valueType: 0,
        value: '-',
        source: 'Hệ thống'
    },
    {
        salaryComponentId: 'sys-6',
        salaryComponentCode: 'TONG_CONG_HUONG_LUONG_THEO_GIO',
        salaryComponentName: 'Tổng công hưởng lương theo giờ',
        salaryComponentSystemId: 'attendance',
        salaryComponentSystemName: 'Chấm công',
        attribute: 0,
        valueType: 0,
        value: '-',
        source: 'Hệ thống'
    }
];

export const mockSystems = [
    { salaryComponentSystemId: 'attendance', salaryComponentSystemName: 'Chấm công' },
    { salaryComponentSystemId: 'kpi', salaryComponentSystemName: 'KPI' },
    { salaryComponentSystemId: 'sales', salaryComponentSystemName: 'Doanh số' },
    { salaryComponentSystemId: 'tax', salaryComponentSystemName: 'Thuế' },
    { salaryComponentSystemId: 'insurance', salaryComponentSystemName: 'Bảo hiểm' }
];

export const mockSalaryCompositionColumns = [
    { dataField: 'componentCode', caption: 'Mã thành phần', visible: true, width: 150, isPinned: true },
    { dataField: 'componentName', caption: 'Tên thành phần', visible: true, width: 250, isPinned: true },
    { dataField: 'appliedUnitName', caption: 'Đơn vị áp dụng', visible: true, width: 200, isPinned: false },
    { dataField: 'salaryComponentSystemName', caption: 'Loại thành phần', visible: true, width: 150, isPinned: false },
    { dataField: 'attribute', caption: 'Tính chất', visible: true, width: 120, isPinned: false },
    { dataField: 'valueType', caption: 'Kiểu giá trị', visible: true, width: 120, isPinned: false },
    { dataField: 'value', caption: 'Giá trị', visible: true, width: 200, isPinned: false },
    { dataField: 'status', caption: 'Trạng thái', visible: true, width: 150, isPinned: false, cellTemplate: 'status-cell' },
    { dataField: 'source', caption: 'Nguồn gốc', visible: true, width: 120, isPinned: false }
];

export const mockSystemCompositionColumns = [
    { dataField: 'componentCode', caption: 'Mã thành phần', visible: true, width: 250, isPinned: true },
    { dataField: 'componentName', caption: 'Tên thành phần', visible: true, width: 250, isPinned: true },
    { dataField: 'salaryComponentSystemName', caption: 'Loại thành phần', visible: true, width: 150, isPinned: false },
    { dataField: 'attribute', caption: 'Tính chất', visible: true, width: 120, isPinned: false },
    { dataField: 'valueType', caption: 'Kiểu giá trị', visible: true, width: 120, isPinned: false },
    { dataField: 'value', caption: 'Giá trị', visible: true, width: 200, isPinned: false },
];

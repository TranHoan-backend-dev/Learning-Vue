-- =============================================
-- Author: Antigravity (AI Assistant)
-- Create date: 2026-05-11
-- Description: Create database and tables for MISA AMIS Salary Components
-- =============================================

-- Create Database
CREATE DATABASE IF NOT EXISTS MISA_AMIS_Salary CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE MISA_AMIS_Salary;

-- 1. Table pa_organization (Danh sách đơn vị công tác)
CREATE TABLE IF NOT EXISTS pa_organization
(
    organization_id   CHAR(36) PRIMARY KEY,
    organization_code VARCHAR(50)  NOT NULL UNIQUE,
    organization_name VARCHAR(255) NOT NULL,
    parent_id         CHAR(36)     DEFAULT NULL,
    created_at        DATETIME     DEFAULT CURRENT_TIMESTAMP,
    created_by        VARCHAR(100) DEFAULT NULL,
    modified_at       DATETIME     DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    modified_by       VARCHAR(100) DEFAULT NULL,
    CONSTRAINT FK_pa_organization_Parent FOREIGN KEY (parent_id) REFERENCES pa_organization (organization_id)
) ENGINE = InnoDB;

-- 2. Table pa_salary_composition_system (Danh mục hệ thống)
CREATE TABLE IF NOT EXISTS pa_salary_composition_system
(
    salary_component_system_id   CHAR(36) PRIMARY KEY,
    salary_component_code      VARCHAR(50)  DEFAULT NULL,
    salary_component_system_name VARCHAR(255) NOT NULL,
    description                  TEXT,
    attribute                  INT          DEFAULT 0,
    value_type                 INT          DEFAULT 0,
    value                      VARCHAR(255) DEFAULT '-',
    created_at                   DATETIME     DEFAULT CURRENT_TIMESTAMP,
    created_by                   VARCHAR(100) DEFAULT NULL,
    modified_at                  DATETIME     DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    modified_by                  VARCHAR(100) DEFAULT NULL
) ENGINE = InnoDB;

-- 3. Table pa_salary_composition (Danh sách Thành phần lương - TPL)
CREATE TABLE IF NOT EXISTS pa_salary_composition
(
    salary_component_id        CHAR(36) PRIMARY KEY,
    salary_component_code      VARCHAR(255) NOT NULL UNIQUE,
    salary_component_name      VARCHAR(255) NOT NULL,
    salary_component_system_id CHAR(36)     NOT NULL,
    attribute                  INT          NOT NULL COMMENT '0: Khác, 1: Thu nhập, 2: Khấu trừ',
    value_type                 INT COMMENT '0: Số, 1: Tiền tệ, 2: Chữ, 3: Ngày, 4: Phần trăm',
    value                      VARCHAR(255) DEFAULT NULL,
    status                     TINYINT(1)   DEFAULT 1 COMMENT '0: Ngừng sử dụng, 1: Đang sử dụng',
    is_used                    TINYINT(1)   DEFAULT 0 COMMENT 'False: Chua them vao Thanh phan luong',
    source                     VARCHAR(50)  DEFAULT 'Tự thêm',
    created_at                 DATETIME     DEFAULT CURRENT_TIMESTAMP,
    created_by                 VARCHAR(100) DEFAULT NULL,
    modified_at                DATETIME     DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    modified_by                VARCHAR(100) DEFAULT NULL,
    CONSTRAINT FK_pa_salary_composition_System FOREIGN KEY (salary_component_system_id) REFERENCES pa_salary_composition_system (salary_component_system_id)
) ENGINE = InnoDB;

-- 3.5 Table pa_salary_composition_organization (Many-to-Many mapping)
CREATE TABLE IF NOT EXISTS pa_salary_composition_organization
(
    salary_component_id CHAR(36) NOT NULL,
    organization_id     CHAR(36) NOT NULL,
    PRIMARY KEY (salary_component_id, organization_id),
    CONSTRAINT FK_sco_salary_component FOREIGN KEY (salary_component_id) REFERENCES pa_salary_composition (salary_component_id) ON DELETE CASCADE,
    CONSTRAINT FK_sco_organization FOREIGN KEY (organization_id) REFERENCES pa_organization (organization_id) ON DELETE CASCADE
) ENGINE = InnoDB;

-- 4. Table pa_grid_config (Danh sách các cột của bảng)
CREATE TABLE IF NOT EXISTS pa_grid_config
(
    grid_config_id CHAR(36) PRIMARY KEY,
    grid_id        VARCHAR(100) NOT NULL COMMENT 'Định danh của grid (vd: SalaryComponentGrid)',
    column_id      VARCHAR(100) NOT NULL COMMENT 'Định danh của cột (vd: salary_component_name)',
    column_name    VARCHAR(255) DEFAULT NULL COMMENT 'Tên hiển thị của cột',
    is_visible     TINYINT(1)   DEFAULT 1,
    column_order   INT          DEFAULT 0,
    width          INT          DEFAULT 150,
    is_pinned      TINYINT(1)   DEFAULT 0,
    pin_side       VARCHAR(10)  DEFAULT 'left' COMMENT 'left hoặc right',
    created_at     DATETIME     DEFAULT CURRENT_TIMESTAMP,
    created_by     VARCHAR(100) DEFAULT NULL,
    modified_at    DATETIME     DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    modified_by    VARCHAR(100) DEFAULT NULL
) ENGINE = InnoDB;

SET FOREIGN_KEY_CHECKS = 0;

Truncate table pa_grid_config;
-- 4. Grid configuration initial setup
INSERT IGNORE INTO pa_grid_config (grid_config_id, grid_id, column_id, column_name, is_visible, column_order, width, is_pinned)
VALUES (UUID(), 'SalaryComponentGrid', 'salary_component_code', 'Mã thành phần', 1, 1, 150, 0),
       (UUID(), 'SalaryComponentGrid', 'salary_component_name', 'Tên thành phần', 1, 2, 250, 1),
       (UUID(), 'SalaryComponentGrid', 'applied_unit_name', 'Đơn vị áp dụng', 1, 3, 200, 0),
       (UUID(), 'SalaryComponentGrid', 'salary_component_system_name', 'Loại thành phần', 1, 4, 150, 0),
       (UUID(), 'SalaryComponentGrid', 'attribute', 'Tính chất', 1, 5, 120, 0),
       (UUID(), 'SalaryComponentGrid', 'value_type', 'Kiểu giá trị', 1, 6, 120, 0),
       (UUID(), 'SalaryComponentGrid', 'value', 'Giá trị', 1, 7, 200, 0),
       (UUID(), 'SalaryComponentGrid', 'source', 'Nguồn tạo', 1, 8, 150, 0),
       (UUID(), 'SalaryComponentGrid', 'status', 'Trạng thái', 1, 9, 150, 0),
       
       -- SalaryComponentSystemGrid (Danh mục hệ thống)
       (UUID(), 'SalaryComponentSystemGrid', 'componentCode', 'Mã thành phần', 1, 1, 250, 1),
       (UUID(), 'SalaryComponentSystemGrid', 'componentName', 'Tên thành phần', 1, 2, 250, 1),
       (UUID(), 'SalaryComponentSystemGrid', 'salaryComponentSystemName', 'Loại thành phần', 1, 3, 150, 0),
       (UUID(), 'SalaryComponentSystemGrid', 'attribute', 'Tính chất', 1, 4, 120, 0),
       (UUID(), 'SalaryComponentSystemGrid', 'valueType', 'Kiểu giá trị', 1, 5, 120, 0),
       (UUID(), 'SalaryComponentSystemGrid', 'value', 'Giá trị', 1, 6, 200, 0);

truncate table pa_salary_composition_organization;
truncate table pa_salary_composition;
-- 3. Seed data for pa_salary_composition (16 records matching the image)
INSERT IGNORE INTO pa_salary_composition (salary_component_id, salary_component_code, salary_component_name,
                                          salary_component_system_id, attribute, value_type, value,
                                          status, source, is_used)
VALUES ('00000000-0000-0000-0002-000000000001', 'CONG_NGAY_NGHI_THEO_GIO', 'Công ngày nghỉ (giờ)', '00000000-0000-0000-0001-000000000006', 0, 0, '-', 1, 'Mặc định', true),
       ('00000000-0000-0000-0002-000000000002', 'LCB', 'LCB', '00000000-0000-0000-0001-000000000009', 1, 1, '5000000', 1, 'Tự thêm', true),
       ('00000000-0000-0000-0002-000000000003', 'BHYT', 'BHYT', '00000000-0000-0000-0001-000000000005', 2, 1, '=BHYT', 1, 'Mặc định', true),
       ('00000000-0000-0000-0002-000000000004', 'BHYT_CONG_TY_DONG', 'BHYT (Công ty đóng)', '00000000-0000-0000-0001-000000000005', 0, 1, '=BHYT_CONG_TY_DONG', 1, 'Mặc định', true),
       ('00000000-0000-0000-0002-000000000005', 'CDP', 'Công đoàn phí', '00000000-0000-0000-0001-000000000005', 2, 1, '=0.01*LUONG_DONG_BH', 1, 'Mặc định', true),
       ('00000000-0000-0000-0002-000000000006', 'CHUC_DANH', 'Chức danh', '00000000-0000-0000-0001-000000000001', 0, 2, '-', 1, 'Mặc định', true),
       ('00000000-0000-0000-0002-000000000007', 'CONG_NGAY_LE_THEO_CA', 'Công ngày lễ (ca)', '00000000-0000-0000-0001-000000000006', 0, 0, '-', 1, 'Mặc định', true),
       ('00000000-0000-0000-0002-000000000008', 'CONG_NGAY_LE_THEO_GIO', 'Công ngày lễ (giờ)', '00000000-0000-0000-0001-000000000006', 0, 0, '-', 1, 'Mặc định', true),
       ('00000000-0000-0000-0002-000000000009', 'CONG_NGAY_LE_THEO_NGAY', 'Công ngày lễ (ngày)', '00000000-0000-0000-0001-000000000006', 0, 0, '-', 1, 'Mặc định', true),
       ('00000000-0000-0000-0002-000000000010', 'CONG_NGAY_NGHI_THEO_CA', 'Công ngày nghỉ (ca)', '00000000-0000-0000-0001-000000000006', 0, 0, '-', 1, 'Mặc định', true),
       ('00000000-0000-0000-0002-000000000011', 'BAC', 'Bậc', '00000000-0000-0000-0001-000000000001', 0, 2, '-', 1, 'Mặc định', true),
       ('00000000-0000-0000-0002-000000000012', 'BAC_LUONG', 'Bậc lương', '00000000-0000-0000-0001-000000000001', 0, 2, '-', 1, 'Mặc định', true),
       ('00000000-0000-0000-0002-000000000013', 'BHTN', 'BHTN', '00000000-0000-0000-0001-000000000005', 2, 1, '=BHTN', 1, 'Mặc định', true),
       ('00000000-0000-0000-0002-000000000014', 'BHTN_CONG_TY_DONG', 'BHTN (Công ty đóng)', '00000000-0000-0000-0001-000000000005', 0, 1, '=BHTN_CONG_TY_DONG', 1, 'Mặc định', true),
       ('00000000-0000-0000-0002-000000000015', 'BHXH', 'BHXH', '00000000-0000-0000-0001-000000000005', 2, 1, '=BHXH', 1, 'Mặc định', true),
       ('00000000-0000-0000-0002-000000000016', 'BHXH_CONG_TY_DONG', 'BHXH (Công ty đóng)', '00000000-0000-0000-0001-000000000005', 0, 1, '=BHXH_CONG_TY_DONG', 1, 'Mặc định', true),
       ('00000000-0000-0000-0002-000000000017', 'CONG_TAC_PHI', 'Công tác phí', '00000000-0000-0000-0001-000000000009', 1, 1, '0', 1, 'Tự thêm', false),
       ('00000000-0000-0000-0002-000000000018', 'DIA_DIEM_LAM_VIEC', 'Địa điểm làm việc', '00000000-0000-0000-0001-000000000006', 0, 2, '-', 1, 'Tự thêm', false),
       ('00000000-0000-0000-0002-000000000019', 'DINH_MUC', 'Định mức', '00000000-0000-0000-0001-000000000003', 0, 1, '-', 1, 'Tự thêm', false),
       ('00000000-0000-0000-0002-000000000020', 'DOANH_SO_MUC_TIEU', 'Doanh số mục tiêu', '00000000-0000-0000-0001-000000000002', 0, 1, '0', 1, 'Tự thêm', false),
       ('00000000-0000-0000-0002-000000000021', 'DOANH_SO_THUC_TE', 'Doanh số thực tế', '00000000-0000-0000-0001-000000000002', 0, 1, '0', 1, 'Tự thêm', false),
       ('00000000-0000-0000-0002-000000000022', 'DON_GIA_CONG_DOAN', 'Đơn giá công đoạn', '00000000-0000-0000-0001-000000000008', 0, 0, '0', 1, 'Tự thêm', false),
       ('00000000-0000-0000-0002-000000000023', 'DON_GIA_SAN_PHAM', 'Đơn giá sản phẩm', '00000000-0000-0000-0001-000000000008', 0, 1, '0', 1, 'Tự thêm', false),
       ('00000000-0000-0000-0002-000000000024', 'DON_GIA_TIEN_LUONG', 'Đơn giá tiền lương', '00000000-0000-0000-0001-000000000001', 0, 1, '0', 1, 'Tự thêm', false),
       ('00000000-0000-0000-0002-000000000025', 'GIAM_TRU_GIA_CANH', 'Giảm trừ gia cảnh', '00000000-0000-0000-0001-0000000000010', 0, 1, '=11000000+4400000*SO_NGUOI_PHU_THUOC', 1, 'Tự thêm', false),
       ('00000000-0000-0000-0002-000000000026', 'GIOI_TINH', 'Giới tính', '00000000-0000-0000-0001-000000000001', 0, 2, '-', 1, 'Tự thêm', false),
       ('00000000-0000-0000-0002-000000000027', 'HE_SO_LUONG', 'Hệ số lương', '00000000-0000-0000-0001-000000000001', 0, 0, '1', 1, 'Tự thêm', false),
       ('00000000-0000-0000-0002-000000000028', 'HE_SO_SAN_PHAM', 'Hệ số sản phẩm', '00000000-0000-0000-0001-000000000008', 0, 0, '1', 1, 'Tự thêm', false),
       ('00000000-0000-0000-0002-000000000029', 'KHEN_THUONG_NHAN_VIEN', 'Khen thưởng nhân viên', '00000000-0000-0000-0001-000000000009', 1, 1, '0', 1, 'Tự thêm', false),
       ('00000000-0000-0000-0002-000000000030', 'KHU_VUC_LAM_VIEC', 'Khu vực làm việc', '00000000-0000-0000-0001-000000000001', 0, 2, '-', 1, 'Tự thêm', false),
       ('00000000-0000-0000-0002-000000000031', 'KPCĐ_CONG_TY_DONG', 'KPCĐ (Công ty đóng)', '00000000-0000-0000-0001-000000000005', 0, 1, '=KPCĐ_CONG_TY_DONG', 1, 'Tự thêm', false),
       ('00000000-0000-0000-0002-000000000032', 'LOAI_LUONG', 'Loại lương', '00000000-0000-0000-0001-000000000001', 0, 2, '-', 1, 'Tự thêm', false),
       ('00000000-0000-0000-0002-000000000033', 'LUONG_CO_BAN', 'Lương cơ bản', '00000000-0000-0000-0001-000000000001', 0, 1, '0', 1, 'Tự thêm', false),
       ('00000000-0000-0000-0002-000000000034', 'THUC_LINH', 'Thực lĩnh', '00000000-0000-0000-0001-000000000009', 0, 1, '=TONG_THU_NHAP-TONG_KHAU_TRU', 1, 'Tự thêm', false);INSERT IGNORE INTO pa_salary_composition_organization (salary_component_id, organization_id)
SELECT salary_component_id, '00000000-0000-0000-0000-000000000001' FROM pa_salary_composition;

-- Thêm các đơn vị áp dụng khác cho ít nhất 10 bản ghi thành phần lương (số lượng đơn vị dao động từ 3-5)
INSERT IGNORE INTO pa_salary_composition_organization (salary_component_id, organization_id)
VALUES 
       -- 1. CONG_NGAY_NGHI_THEO_GIO (Tổng cộng 3 đơn vị)
       ('00000000-0000-0000-0002-000000000001', '00000000-0000-0000-0000-000000000002'),
       ('00000000-0000-0000-0002-000000000001', '00000000-0000-0000-0000-000000000003'),
       
       -- 2. LCB (Tổng cộng 4 đơn vị)
       ('00000000-0000-0000-0002-000000000002', '00000000-0000-0000-0000-000000000002'),
       ('00000000-0000-0000-0002-000000000002', '00000000-0000-0000-0000-000000000003'),
       ('00000000-0000-0000-0002-000000000002', '00000000-0000-0000-0000-000000000004'),
       
       -- 3. BHYT (Tổng cộng 5 đơn vị)
       ('00000000-0000-0000-0002-000000000003', '00000000-0000-0000-0000-000000000002'),
       ('00000000-0000-0000-0002-000000000003', '00000000-0000-0000-0000-000000000003'),
       ('00000000-0000-0000-0002-000000000003', '00000000-0000-0000-0000-000000000004'),
       ('00000000-0000-0000-0002-000000000003', '00000000-0000-0000-0000-000000000005'),
       
       -- 4. BHYT_CONG_TY_DONG (Tổng cộng 3 đơn vị)
       ('00000000-0000-0000-0002-000000000004', '00000000-0000-0000-0000-000000000005'),
       ('00000000-0000-0000-0002-000000000004', '00000000-0000-0000-0000-000000000006'),
       
       -- 5. CDP (Tổng cộng 4 đơn vị)
       ('00000000-0000-0000-0002-000000000005', '00000000-0000-0000-0000-000000000006'),
       ('00000000-0000-0000-0002-000000000005', '00000000-0000-0000-0000-000000000007'),
       ('00000000-0000-0000-0002-000000000005', '00000000-0000-0000-0000-000000000008'),
       
       -- 6. CHUC_DANH (Tổng cộng 5 đơn vị)
       ('00000000-0000-0000-0002-000000000006', '00000000-0000-0000-0000-000000000008'),
       ('00000000-0000-0000-0002-000000000006', '00000000-0000-0000-0000-000000000009'),
       ('00000000-0000-0000-0002-000000000006', '00000000-0000-0000-0000-000000000010'),
       ('00000000-0000-0000-0002-000000000006', '00000000-0000-0000-0000-000000000011'),
       
       -- 7. CONG_NGAY_LE_THEO_CA (Tổng cộng 3 đơn vị)
       ('00000000-0000-0000-0002-000000000007', '00000000-0000-0000-0000-000000000003'),
       ('00000000-0000-0000-0002-000000000007', '00000000-0000-0000-0000-000000000005'),
       
       -- 8. CONG_NGAY_LE_THEO_GIO (Tổng cộng 4 đơn vị)
       ('00000000-0000-0000-0002-000000000008', '00000000-0000-0000-0000-000000000002'),
       ('00000000-0000-0000-0002-000000000008', '00000000-0000-0000-0000-000000000004'),
       ('00000000-0000-0000-0002-000000000008', '00000000-0000-0000-0000-000000000006'),
       
       -- 9. CONG_NGAY_LE_THEO_NGAY (Tổng cộng 5 đơn vị)
       ('00000000-0000-0000-0002-000000000009', '00000000-0000-0000-0000-000000000003'),
       ('00000000-0000-0000-0002-000000000009', '00000000-0000-0000-0000-000000000005'),
       ('00000000-0000-0000-0002-000000000009', '00000000-0000-0000-0000-000000000007'),
       ('00000000-0000-0000-0002-000000000009', '00000000-0000-0000-0000-000000000009'),
       
       -- 10. CONG_NGAY_NGHI_THEO_CA (Tổng cộng 3 đơn vị)
       ('00000000-0000-0000-0002-000000000010', '00000000-0000-0000-0000-000000000011'),
       ('00000000-0000-0000-0002-000000000010', '00000000-0000-0000-0000-000000000012');

truncate table pa_salary_composition_system;
-- 2. System Components with fixed IDs for seeding
INSERT IGNORE INTO pa_salary_composition_system (salary_component_system_id, salary_component_code, salary_component_system_name, attribute, value_type, value)
VALUES ('00000000-0000-0000-0001-000000000001', 'TTNV', 'Thông tin nhân viên', 0, 2, '-'),
       ('00000000-0000-0000-0001-000000000006', 'CHAM_CONG', 'Chấm công', 0, 0, '0'),
       ('00000000-0000-0000-0001-000000000002', 'DOANH_SO', 'Doanh số', 1, 1, '0'),
       ('00000000-0000-0000-0001-000000000007', 'KPI', 'KPI', 0, 0, '0'),
       ('00000000-0000-0000-0001-000000000008', 'SAN_PHAM', 'Sản phẩm', 0, 0, '0'),
       ('00000000-0000-0000-0001-000000000009', 'LUONG', 'Lương', 0, 0, '0'),
       ('00000000-0000-0000-0001-0000000000010', 'THUE_TNCN', 'Thuế TNCN', 0, 0, '0'),
       ('00000000-0000-0000-0001-000000000005', 'BH_CD', 'Bảo hiểm - Công đoàn', 2, 1, '0'),
       ('00000000-0000-0000-0001-000000000003', 'KHAC', 'Khác', 0, 2, '-');

truncate table pa_organization;
-- Organization Hierarchy
INSERT IGNORE INTO pa_organization (organization_id, organization_code, organization_name, parent_id)
VALUES ('00000000-0000-0000-0000-000000000001', 'HOP_NHAT', 'Cty CP TM dịch vụ Hợp Nhất', NULL),
       -- Children of Hợp Nhất
       ('00000000-0000-0000-0000-000000000002', 'CN_BAC', 'Chi nhánh phía Bắc', '00000000-0000-0000-0000-000000000001'),
       ('00000000-0000-0000-0000-000000000003', 'CN_NAM', 'Chi nhánh phía Nam', '00000000-0000-0000-0000-000000000001'),
       ('00000000-0000-0000-0000-000000000004', 'CN_MT', 'Chi nhánh miền Tây', '00000000-0000-0000-0000-000000000001'),
       ('00000000-0000-0000-0000-000000000005', 'TT_SX', 'Trung tâm sản xuất', '00000000-0000-0000-0000-000000000001'),
       -- Children of Chi nhánh phía Bắc
       ('00000000-0000-0000-0000-000000000006', 'VP_HN', 'Văn phòng Hà Nội', '00000000-0000-0000-0000-000000000002'),
       ('00000000-0000-0000-0000-000000000007', 'VP_LS', 'Văn phòng Lạng Sơn', '00000000-0000-0000-0000-000000000002'),
       -- Children of Chi nhánh phía Nam
       ('00000000-0000-0000-0000-000000000008', 'VP_HCM', 'Văn phòng Hồ Chí Minh', '00000000-0000-0000-0000-000000000003'),
       ('00000000-0000-0000-0000-000000000009', 'CN_CT', 'Chi nhánh Cần Thơ', '00000000-0000-0000-0000-000000000003'),
       -- Children of Trung tâm sản xuất
       ('00000000-0000-0000-0000-000000000010', 'KH_DN', 'Khối Doanh nghiệp', '00000000-0000-0000-0000-000000000005'),
       ('00000000-0000-0000-0000-000000000011', 'KH_GD', 'Khối nền tảng Giáo dục', '00000000-0000-0000-0000-000000000005'),
       ('00000000-0000-0000-0000-000000000012', 'KH_GPBL', 'Khối Giải pháp bán lẻ', '00000000-0000-0000-0000-000000000005'),
       ('00000000-0000-0000-0000-000000000013', 'BAN_CNTT', 'Ban Công nghệ thông tin', '00000000-0000-0000-0000-000000000005');

SET FOREIGN_KEY_CHECKS = 1;

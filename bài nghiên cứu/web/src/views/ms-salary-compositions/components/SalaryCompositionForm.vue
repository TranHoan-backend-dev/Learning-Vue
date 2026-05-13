<script setup lang="ts">
import { ref, onMounted, watch, nextTick, computed } from 'vue';
import DxSelectBox, { DxButton } from 'devextreme-vue/select-box';
import MsFormula from '@/components/ui/ms-formula/MsFormula.vue';
import organizationService from '@/services/organizationService.ts';
import salaryCompositionSystemService from '@/services/salaryCompositionSystemService.ts';
import {attributeOptions, valueTypeOptions} from "@/views/ms-salary-compositions/data.ts";

const emit = defineEmits(['close', 'save']);
const props = defineProps<{
  mode: 'add' | 'edit' | 'copy';
  initialData?: any;
}>();

// ============================================================
// 1. Form Data & Defaults
// ============================================================
const getDefaultData = () => ({
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
});

const formData = ref(getDefaultData());

// ============================================================
// 2. Refs cho auto-focus & tab navigation
// ============================================================
const componentNameRef = ref<HTMLInputElement | null>(null);
const componentIdRef = ref<HTMLInputElement | null>(null);
const quotaFormulaRef = ref<InstanceType<typeof MsFormula> | null>(null);
const valueFormulaRef = ref<InstanceType<typeof MsFormula> | null>(null);

// ============================================================
// 3. Validation
// ============================================================
const errors = ref<Record<string, string>>({});

const validateField = (field: string): boolean => {
  const value = (formData.value as any)[field];

  switch (field) {
    case 'componentName':
      if (!value || !value.trim()) {
        errors.value.componentName = 'Tên thành phần không được để trống';
        return false;
      }
      if (value.length > 255) {
        errors.value.componentName = 'Tên thành phần không được vượt quá 255 ký tự';
        return false;
      }
      break;

    case 'componentId':
      if (!value || !value.trim()) {
        errors.value.componentId = 'Mã thành phần không được để trống';
        return false;
      }
      if (value.length > 255) {
        errors.value.componentId = 'Mã thành phần không nên quá dài';
        return false;
      }
      // Kiểm tra định dạng: chỉ chữ, số, gạch dưới
      if (!/^[A-Za-z0-9_]+$/.test(value)) {
        errors.value.componentId = 'Mã thành phần chỉ gồm chữ, số và dấu gạch dưới';
        return false;
      }
      break;

    case 'salaryComponentSystemId':
      if (!value) {
        errors.value.salaryComponentSystemId = 'Loại thành phần không được để trống';
        return false;
      }
      break;

    case 'attribute':
      if (value === null || value === undefined) {
        errors.value.attribute = 'Tính chất không được để trống';
        return false;
      }
      break;
  }

  // Xóa lỗi nếu hợp lệ
  delete errors.value[field];
  return true;
};

const validateAll = (): boolean => {
  const fields = ['componentName', 'componentId', 'salaryComponentSystemId', 'attribute'];
  let isValid = true;

  // Validate tất cả các trường
  for (const field of fields) {
    if (!validateField(field)) {
      isValid = false;
    }
  }

  return isValid;
};

// Focus vào ô lỗi đầu tiên
const focusFirstError = () => {
  nextTick(() => {
    const errorFields = ['componentName', 'componentId', 'salaryComponentSystemId', 'attribute'];
    for (const field of errorFields) {
      if (errors.value[field]) {
        switch (field) {
          case 'componentName':
            componentNameRef.value?.focus();
            return;
          case 'componentId':
            componentIdRef.value?.focus();
            return;
          case 'salaryComponentSystemId':
          case 'attribute':
            // DxSelectBox — tìm input bên trong qua DOM
            const el = document.querySelector(`[data-field="${field}"] .dx-texteditor-input`) as HTMLElement;
            el?.focus();
            return;
        }
      }
    }
  });
};

// Xóa lỗi khi user sửa trường
const clearError = (field: string) => {
  delete errors.value[field];
};

// ============================================================
// 4. Xử lý giao diện khi thay đổi "Tính chất"
// ============================================================
const showTaxOptions = computed(() => {
  return formData.value.attribute === 1 || formData.value.attribute === 2;
});

const attributeLabel = computed(() => {
  if (formData.value.attribute === 1) return 'Thu nhập';
  if (formData.value.attribute === 2) return 'Khấu trừ';
  return 'Khác';
});

watch(() => formData.value.attribute, (newVal, oldVal) => {
  if (newVal !== oldVal) {
    // Khi attribute thay đổi, reset các giá trị liên quan
    if (newVal === 2) {
      // Khấu trừ: mặc định không có radio thuế
      formData.value.taxType = null;
    } else if (newVal === 1) {
      // Thu nhập: mặc định Chịu thuế
      formData.value.taxType = 'Chịu thuế';
    }
  }
});

// ============================================================
// 5. Form Init
// ============================================================
const initForm = () => {
  errors.value = {};
  if (props.mode === 'edit' || props.mode === 'copy') {
    if (props.initialData) {
      formData.value = {
        ...getDefaultData(),
        ...props.initialData,
        salaryComponentSystemId: props.initialData.salaryComponentSystemId,
        appliedUnitId: props.initialData.appliedUnitId
      };

      if (props.initialData.value) {
        formData.value.valueFormula = props.initialData.value;
      }

      if (props.mode === 'copy') {
        formData.value.componentId = `${formData.value.componentId}_COPY`;
      }
    }
  } else {
    formData.value = getDefaultData();
  }
};

const appliedUnits = ref<any[]>([]);
const salaryComponentSystems = ref<any[]>([]);

const loadCategories = async () => {
  try {
    const [unitsRes, systemsRes] = await Promise.all([
      organizationService.getAll(),
      salaryCompositionSystemService.getAll()
    ]);
    appliedUnits.value = unitsRes.data;
    salaryComponentSystems.value = systemsRes.data;

    // Nếu là add, set mặc định cho system nếu có
    if (props.mode === 'add' && salaryComponentSystems.value.length > 0) {
      formData.value.salaryComponentSystemId = salaryComponentSystems.value[0].salaryComponentSystemId;
    }
  } catch (error) {
    console.error('Lỗi khi tải danh mục:', error);
  }
};

onMounted(() => {
  initForm();
  loadCategories();
  // Auto-focus vào ô input đầu tiên
  nextTick(() => {
    componentNameRef.value?.focus();
  });
});

watch(() => props.initialData, () => {
  initForm();
  nextTick(() => componentNameRef.value?.focus());
}, { deep: true });

watch(() => props.mode, () => {
  initForm();
  nextTick(() => componentNameRef.value?.focus());
});

// ============================================================
// 6. Save handlers
// ============================================================
const handleSave = () => {
  if (!validateAll()) {
    focusFirstError();
    return;
  }
  emit('save', formData.value);
  emit('close');
};

const handleSaveAndAdd = async () => {
  if (!validateAll()) {
    focusFirstError();
    return;
  }
  emit('save', formData.value);
  // Reset form cho lần thêm tiếp theo
  formData.value = getDefaultData();
  errors.value = {};
  nextTick(() => componentNameRef.value?.focus());
};
const calculationTargetOptions = ['Trong cùng đơn vị công tác', 'Toàn công ty'];
</script>

<template>
  <div class="salary-form-overlay">
    <div class="salary-form-container">
      <div class="salary-form-header">
        <div class="salary-form-header-left">
          <div class="back-btn" @click="$emit('close')" title="Quay lại">
            <svg viewBox="0 0 24 24" width="24" height="24" fill="none" stroke="#666" stroke-width="2"
              stroke-linecap="round" stroke-linejoin="round">
              <line x1="19" y1="12" x2="5" y2="12"></line>
              <polyline points="12 19 5 12 12 5"></polyline>
            </svg>
          </div>
          <div class="salary-form-title">{{ mode === 'edit' ? 'Sửa' : 'Thêm' }} thành phần</div>
        </div>
        <div class="salary-form-header-right">
          <button class="misa-btn-cancel" @click="$emit('close')">Hủy bỏ</button>
          <button class="misa-btn-outline" @click="handleSaveAndAdd">Lưu và thêm</button>
          <button class="misa-btn-primary" @click="handleSave">Lưu</button>
        </div>
      </div>

      <!-- Body -->
      <div class="salary-form-body">
        <div class="form-wrapper">
          <!-- Tên thành phần -->
          <div class="form-row" :class="{ 'has-error': errors.componentName }">
            <div class="form-label">Tên thành phần <span class="required">*</span></div>
            <div class="form-control">
              <input ref="componentNameRef" type="text" class="misa-input w-full-input"
                :class="{ 'input-error': errors.componentName }" v-model="formData.componentName"
                @input="clearError('componentName')" @blur="validateField('componentName')" maxlength="255"
                tabindex="1" />
              <div v-if="errors.componentName" class="error-message">{{ errors.componentName }}</div>
            </div>
          </div>

          <!-- Mã thành phần -->
          <div class="form-row" :class="{ 'has-error': errors.componentId }">
            <div class="form-label">Mã thành phần <span class="required">*</span></div>
            <div class="form-control">
              <input ref="componentIdRef" type="text" class="misa-input w-full-input"
                :class="{ 'input-error': errors.componentId }" v-model="formData.componentId"
                placeholder="Nhập mã viết liền" @input="clearError('componentId')" @blur="validateField('componentId')"
                maxlength="255" tabindex="2" />
              <div v-if="errors.componentId" class="error-message">{{ errors.componentId }}</div>
            </div>
          </div>

          <!-- Đơn vị áp dụng -->
          <div class="form-row">
            <div class="form-label">Đơn vị áp dụng</div>
            <div class="form-control">
              <DxSelectBox class="misa-selectbox w-full-input" :items="appliedUnits" display-expr="organizationName"
                value-expr="organizationId" v-model="formData.appliedUnitId" placeholder="--- Tất cả đơn vị ---"
                :tab-index="3" />
            </div>
          </div>

          <!-- Loại thành phần -->
          <div class="form-row" :class="{ 'has-error': errors.salaryComponentSystemId }">
            <div class="form-label">Loại thành phần <span class="required">*</span></div>
            <div class="form-control" data-field="salaryComponentSystemId">
              <DxSelectBox class="misa-selectbox w-full-input"
                :class="{ 'select-error': errors.salaryComponentSystemId }" :items="salaryComponentSystems"
                display-expr="salaryComponentSystemName" value-expr="salaryComponentSystemId"
                v-model="formData.salaryComponentSystemId" @value-changed="clearError('salaryComponentSystemId')"
                :tab-index="4" />
              <div v-if="errors.salaryComponentSystemId" class="error-message">{{ errors.salaryComponentSystemId }}
              </div>
            </div>
          </div>

          <!-- Tính chất -->
          <div class="form-row" :class="{ 'has-error': errors.attribute }">
            <div class="form-label">Tính chất <span class="required">*</span></div>
            <div class="form-control flex-row" data-field="attribute">
              <DxSelectBox class="misa-selectbox" :class="{ 'select-error': errors.attribute }" style="width: 250px"
                :items="attributeOptions" display-expr="name" value-expr="id" v-model="formData.attribute"
                @value-changed="clearError('attribute')" :tab-index="5" />

              <!-- Radio thuế: chỉ hiển thị khi Tính chất = Thu nhập -->
              <div v-if="showTaxOptions && formData.attribute === 1" class="radio-group ml-24">
                <label class="radio-label">
                  <input type="radio" v-model="formData.taxType" value="Chịu thuế" tabindex="6">
                  <span class="radio-custom"></span> Chịu thuế
                </label>
                <label class="radio-label">
                  <input type="radio" v-model="formData.taxType" value="Miễn thuế toàn phần" tabindex="7">
                  <span class="radio-custom"></span> Miễn thuế toàn phần
                </label>
                <label class="radio-label">
                  <input type="radio" v-model="formData.taxType" value="Miễn thuế một phần" tabindex="8">
                  <span class="radio-custom"></span> Miễn thuế một phần
                </label>
              </div>
            </div>
            <div v-if="errors.attribute" class="error-message" style="margin-left: 220px;">{{ errors.attribute }}</div>
          </div>

          <!-- Định mức -->
          <div class="form-row align-top">
            <div class="form-label pt-8">Định mức</div>
            <div class="form-control">
              <div class="w-full-input">
                <MsFormula ref="quotaFormulaRef" v-model="formData.quota"
                  placeholder="Tự động gợi ý công thức và tham số khi gõ" :rows="6" />
              </div>
              <div class="checkbox-container mt-16">
                <label class="checkbox-label">
                  <input type="checkbox" v-model="formData.allowExceedQuota" tabindex="10">
                  <span class="checkbox-custom"></span> Cho phép giá trị tính vượt quá định mức
                </label>
                <div class="info-icon-container ml-4" title="Giải thích về định mức">
                  <svg width="16" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <path
                      d="M8 14.6667C11.6819 14.6667 14.6667 11.6819 14.6667 8C14.6667 4.3181 11.6819 1.33334 8 1.33334C4.3181 1.33334 1.33334 4.3181 1.33334 8C1.33334 11.6819 4.3181 14.6667 8 14.6667Z"
                      stroke="#0075FF" stroke-width="1.33333" stroke-linecap="round" stroke-linejoin="round" />
                    <path d="M8 5.33334V5.34001" stroke="#0075FF" stroke-width="1.33333" stroke-linecap="round"
                      stroke-linejoin="round" />
                    <path d="M8 10.6667V8" stroke="#0075FF" stroke-width="1.33333" stroke-linecap="round"
                      stroke-linejoin="round" />
                  </svg>
                </div>
              </div>
            </div>
          </div>

          <!-- Kiểu giá trị -->
          <div class="form-row">
            <div class="form-label">Kiểu giá trị</div>
            <div class="form-control">
              <DxSelectBox class="misa-selectbox misa-selectbox-gray" style="width: 250px" :items="valueTypeOptions"
                display-expr="name" value-expr="id" v-model="formData.valueType" :tab-index="11" />
            </div>
          </div>

          <!-- Giá trị -->
          <div class="form-row align-top">
            <div class="form-label pt-8">Giá trị</div>
            <div class="form-control flex-col">
              <div class="radio-row mb-12">
                <label class="radio-label">
                  <input type="radio" v-model="formData.valueCalculation" value="Tự động cộng tổng" tabindex="12">
                  <span class="radio-custom"></span> Tự động cộng tổng giá trị của các nhân viên
                </label>
                <div class="inline-selectbox-wrapper ml-12">
                  <DxSelectBox class="misa-selectbox misa-selectbox-gray" style="width: 250px"
                    :items="calculationTargetOptions" v-model="formData.valueCalculationTarget"
                    :disabled="formData.valueCalculation !== 'Tự động cộng tổng'" :tab-index="13">
                    <DxButton name="info" location="after" :options="{
                      icon: 'info',
                      type: 'default',
                      stylingMode: 'text',
                      elementAttr: { class: 'info-button-inside' }
                    }" />
                    <DxButton name="dropDown" />
                  </DxSelectBox>
                </div>
              </div>

              <div class="radio-row mb-12">
                <label class="radio-label">
                  <input type="radio" v-model="formData.valueCalculation" value="Tính theo công thức tự đặt"
                    tabindex="14">
                  <span class="radio-custom"></span> Tính theo công thức tự đặt
                </label>
              </div>

              <div class="w-full-input">
                <MsFormula ref="valueFormulaRef" v-model="formData.valueFormula"
                  placeholder="Tự động gợi ý công thức và tham số khi gõ"
                  :disabled="formData.valueCalculation !== 'Tính theo công thức tự đặt'" :rows="6" />
              </div>
            </div>
          </div>

          <!-- Mô tả -->
          <div class="form-row align-top mt-24">
            <div class="form-label pt-8">Mô tả</div>
            <div class="form-control">
              <textarea class="misa-textarea w-full-input" rows="3" v-model="formData.description"
                tabindex="15"></textarea>
            </div>
          </div>

          <!-- Hiển thị trên phiếu lương -->
          <div class="form-row mt-24">
            <div class="form-label">Hiển thị trên phiếu lương</div>
            <div class="form-control">
              <div class="radio-group">
                <label class="radio-label">
                  <input type="radio" v-model="formData.showOnPayslip" value="Có" tabindex="16">
                  <span class="radio-custom"></span> Có
                </label>
                <label class="radio-label">
                  <input type="radio" v-model="formData.showOnPayslip" value="Không" tabindex="17">
                  <span class="radio-custom"></span> Không
                </label>
                <label class="radio-label">
                  <input type="radio" v-model="formData.showOnPayslip" value="Chỉ hiển thị nếu giá trị khác 0"
                    tabindex="18">
                  <span class="radio-custom"></span> Chỉ hiển thị nếu giá trị khác 0
                </label>
              </div>
            </div>
          </div>

          <!-- Nguồn tạo -->
          <div class="form-row">
            <div class="form-label">Nguồn tạo</div>
            <div class="form-control pb-12 border-bottom">
              <span class="label-value">{{ formData.sourceType }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.salary-form-overlay {
  width: 100%;
  height: 100%;
  background-color: #f4f5f8;
  display: flex;
  flex-direction: column;
}

.salary-form-container {
  width: 100%;
  height: 100%;
  background: #fff;
  display: flex;
  flex-direction: column;
}

/* Header */
.salary-form-header {
  height: 60px;
  min-height: 60px;
  padding: 0 24px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-bottom: 1px solid #e0e0e0;
}

.salary-form-header-left {
  display: flex;
  align-items: center;
  gap: 16px;
}

.back-btn {
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
}

.salary-form-title {
  font-size: 20px;
  font-weight: 700;
  color: #111;
}

.salary-form-header-right {
  display: flex;
  gap: 12px;
}

.misa-btn-cancel,
.misa-btn-outline,
.misa-btn-primary {
  padding: 8px 24px;
  border-radius: 4px;
  font-weight: 600;
  cursor: pointer;
  font-family: inherit;
  font-size: 14px;
  transition: all 0.2s;
}

.misa-btn-cancel {
  background: #fff;
  border: 1px solid transparent;
  color: #111;
}

.misa-btn-cancel:hover {
  background: #f5f5f5;
}

.misa-btn-outline {
  background: #fff;
  border: 1px solid #e0e0e0;
  color: #111;
}

.misa-btn-outline:hover {
  background: #f5f5f5;
}

.misa-btn-primary {
  background: #2ca01c;
  border: 1px solid transparent;
  color: #fff;
}

.misa-btn-primary:hover {
  background: #248b17;
}

/* Body */
.salary-form-body {
  flex: 1;
  overflow-y: auto;
  padding: 24px;
  background-color: #fff;
}

.form-wrapper {
  width: 100%;
}

.form-row {
  display: flex;
  margin-bottom: 24px;
  align-items: center;
  flex-wrap: wrap;
}

.form-row.align-top {
  align-items: flex-start;
}

.form-label {
  width: 180px;
  min-width: 180px;
  font-size: 14px;
  color: #111;
  font-weight: 500;
}

.pt-8 {
  padding-top: 8px;
}

.required {
  color: #ff4d4f;
}

.form-control {
  flex: 1;
}

.w-full {
  width: 100%;
}

.w-full-input {
  width: 750px;
  max-width: 100%;
}

/* Input & Textarea */
.misa-input,
.misa-textarea {
  padding: 8px 12px;
  border: 1px solid #e0e0e0;
  border-radius: 4px;
  font-family: inherit;
  font-size: 14px;
  color: #111;
  outline: none;
  transition: border-color 0.2s;
  box-sizing: border-box;
}

.misa-input:focus,
.misa-textarea:focus {
  border-color: #2ca01c;
}

.misa-textarea {
  resize: vertical;
}

/* Validation error styles */
.misa-input.input-error {
  border-color: #ff4d4f;
}

.misa-input.input-error:focus {
  border-color: #ff4d4f;
  box-shadow: 0 0 0 2px rgba(255, 77, 79, 0.1);
}

:deep(.select-error .dx-texteditor-container) {
  border-color: #ff4d4f !important;
}

.error-message {
  color: #ff4d4f;
  font-size: 12px;
  margin-top: 4px;
  line-height: 1.4;
}

.flex-row {
  display: flex;
  align-items: center;
}

.flex-col {
  display: flex;
  flex-direction: column;
}

.ml-24 {
  margin-left: 24px;
}

.ml-12 {
  margin-left: 12px;
}

.ml-8 {
  margin-left: 8px;
}

.mb-12 {
  margin-bottom: 12px;
}

.mb-8 {
  margin-bottom: 8px;
}

.mt-24 {
  margin-top: 24px;
}

.pt-24 {
  padding-top: 24px;
}

.pb-24 {
  padding-bottom: 24px;
}

.pb-12 {
  padding-bottom: 12px;
}

.border-top {
  border-top: 1px solid #e0e0e0;
}

.border-bottom {
  border-bottom: 1px solid #e0e0e0;
}

.mt-16 {
  margin-top: 16px;
}

.mt-8 {
  margin-top: 8px;
}

.radio-group,
.radio-row {
  display: flex;
  align-items: center;
}

.radio-group {
  gap: 32px;
}

.inline-selectbox-wrapper {
  display: flex;
  align-items: center;
}

.checkbox-container {
  display: flex;
  align-items: center;
}

.label-value {
  font-size: 14px;
  color: #111;
}

/* Custom Radio & Checkbox */
.radio-label,
.checkbox-label {
  display: flex;
  align-items: center;
  cursor: pointer;
  font-size: 14px;
  color: #111;
}

.radio-label input,
.checkbox-label input {
  display: none;
}

.radio-custom,
.checkbox-custom {
  width: 18px;
  height: 18px;
  border: 1px solid #afafaf;
  margin-right: 8px;
  display: inline-block;
  position: relative;
  background-color: #fff;
  transition: all 0.2s;
}

.radio-custom {
  border-radius: 50%;
}

.checkbox-custom {
  border-radius: 3px;
}

.radio-label:hover .radio-custom,
.checkbox-label:hover .checkbox-custom {
  border-color: #2ca01c;
}

.radio-label input:checked+.radio-custom {
  border-color: #2ca01c;
}

.radio-label input:checked+.radio-custom::after {
  content: "";
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 10px;
  height: 10px;
  background-color: #2ca01c;
  border-radius: 50%;
}

.checkbox-label input:checked+.checkbox-custom {
  border-color: #2ca01c;
  background-color: #fff;
}

.checkbox-label input:checked+.checkbox-custom::after {
  content: "";
  position: absolute;
  left: 6px;
  top: 2px;
  width: 5px;
  height: 10px;
  border: solid #2ca01c;
  border-width: 0 2px 2px 0;
  transform: rotate(45deg);
}

.info-icon-container {
  display: flex;
  align-items: center;
  cursor: help;
}

.info-icon {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 16px;
  height: 16px;
  border-radius: 50%;
  border: 1px solid #888;
  color: #888;
  font-size: 10px;
  margin-left: 4px;
}

.misa-input.w-full-input {
  width: 750px !important;
}

:deep(.dx-selectbox) {
  border-radius: 4px;
}

:deep(.dx-texteditor.dx-editor-outlined) {
  border-radius: 4px;
  border: 1px solid #e0e0e0;
}

:deep(.dx-texteditor.dx-state-focused.dx-editor-outlined) {
  border-color: #2ca01c;
}

:deep(.misa-selectbox-gray .dx-texteditor-container) {
  background-color: #f2f2f2 !important;
  border-color: transparent !important;
}

:deep(.misa-selectbox-gray .dx-texteditor-input) {
  color: #111 !important;
}

:deep(.info-button-inside .dx-icon) {
  color: #0075FF !important;
  font-size: 16px !important;
}
</style>

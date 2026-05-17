<script setup lang="ts">
import {ref, onMounted, watch, nextTick, computed} from 'vue';
import DxDropDownBox from 'devextreme-vue/drop-down-box';
import DxTreeView from 'devextreme-vue/tree-view';
import DxSelectBox, {DxButton} from 'devextreme-vue/select-box';
import MsFormula from '@/components/ui/ms-formula/MsFormula.vue';
import organizationService from '@/services/organizationService.ts';
import salaryCompositionSystemService from '@/services/salaryCompositionSystemService.ts';
import {
  calculationTargetOptions,
  defaultData, showOnPayslipOptions,
  taxOptions,
} from "@/views/ms-salary-compositions/components/form/data.ts";
import InfoIcon from "@/components/ui/ms-icon/InfoIcon.vue";
import {attributeOptions, valueTypeOptions} from "@/views/ms-salary-compositions/data.ts";

const emit = defineEmits(['close', 'save']);
const props = defineProps<{
  mode: 'add' | 'edit' | 'copy' | 'view';
  initialData?: any;
}>();

// <editor-fold> desc="Khoi tao cau truc du lieu"
const getDefaultData = () => ({...defaultData});

const formData = ref(getDefaultData());
// </editor-fold>

// Refs cho auto-focus & tab navigation
const componentNameRef = ref<HTMLInputElement | null>(null);
const componentCodeRef = ref<HTMLInputElement | null>(null);
const quotaFormulaRef = ref<InstanceType<typeof MsFormula> | null>(null);
const valueFormulaRef = ref<InstanceType<typeof MsFormula> | null>(null);

// <editor-fold> desc="Xu ly tu dong dien cho Ma thanh phan"
// Auto-fill code logic
const isCodeManuallyEdited = ref(false);

const removeAccentsAndSpecialChars = (str: string): string => {
  if (!str) return '';
  let result = str.toLowerCase();
  result = result.replace(/[àáạảãâầấậẩẫăằắặẳẵ]/g, 'a');
  result = result.replace(/[èéẹẻẽêềếệểễ]/g, 'e');
  result = result.replace(/[ìíịỉĩ]/g, 'i');
  result = result.replace(/[òóọỏõôồốộổỗơờớợởỡ]/g, 'o');
  result = result.replace(/[ùúụủũưừứựửữ]/g, 'u');
  result = result.replace(/[ỳýỵỷỹ]/g, 'y');
  result = result.replace(/đ/g, 'd');
  result = result.replace(/[^a-z0-9]/g, ' ');
  result = result.trim().replace(/\s+/g, '_');
  return result.toUpperCase();
};

const onComponentNameInput = () => {
  clearError('componentName');
  if (props.mode === 'add' || props.mode === 'copy' || props.mode === 'edit') {
    if (!isCodeManuallyEdited.value) {
      formData.value.componentCode = removeAccentsAndSpecialChars(formData.value.componentName);
      if (formData.value.componentCode) {
        clearError('componentCode');
      }
    }
  }
};

const onComponentCodeInput = () => {
  clearError('componentCode');
  isCodeManuallyEdited.value = true;
  if (!formData.value.componentCode) {
    isCodeManuallyEdited.value = false;
  }
};
// </editor-fold>

// Validation
const errors = ref<Record<string, string>>({});

// <editor-fold> desc="Validation"
/**
 * Validate cac thuoc tinh trong form
 * ComponentName: khong duoc de trong, khong dai qua 255 ky tu
 * ComponentCode: khong duoc de trong, khong dai qua 255 ky tu, khong ky tu dac biet (chỉ có chu, số, dấu gạch dưới)
 * SalaryComponentSystemId: khong duoc de trong
 * Attribute: khong duoc de trong
 * @param field
 */
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

    case 'componentCode':
      if (!value || !value.trim()) {
        errors.value.componentCode = 'Mã thành phần không được để trống';
        return false;
      }
      if (value.length > 255) {
        errors.value.componentCode = 'Mã thành phần không nên quá dài';
        return false;
      }
      // Kiểm tra định dạng: chỉ chữ, số, gạch dưới
      if (!/^[A-Za-z0-9_]+$/.test(value)) {
        errors.value.componentCode = 'Mã thành phần chỉ gồm chữ, số và dấu gạch dưới';
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

    case 'appliedFor':
      if (!treeBoxValue.value || treeBoxValue.value.length === 0) {
        errors.value.appliedFor = 'Đơn vị áp dụng không được để trống'
        return false;
      }
      break;
  }

  // Xóa lỗi nếu hợp lệ
  delete errors.value[field];
  return true;
};

const validateAll = (): boolean => {
  const fields = ['componentName', 'componentCode', 'salaryComponentSystemId', 'attribute', 'appliedFor'];
  let isValid = true;

  // Validate tất cả các trường
  for (const field of fields) {
    if (!validateField(field)) {
      isValid = false;
    }
  }

  return isValid;
};
// </editor-fold>

// Focus vào ô lỗi đầu tiên
const focusFirstError = () => {
  nextTick(() => {
    const errorFields = ['componentName', 'componentCode', 'salaryComponentSystemId', 'attribute'];
    for (const field of errorFields) {
      if (errors.value[field]) {
        switch (field) {
          case 'componentName':
            componentNameRef.value?.focus();
            return;
          case 'componentCode':
            componentCodeRef.value?.focus();
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

// Xử lý giao diện khi thay đổi "Tính chất"
const showTaxOptions = computed(() => {
  return formData.value.attribute === 1 || formData.value.attribute === 2;
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

// Khởi tạo form
/**
 * Co 3 che do la edit, copy, view. Edit/View thi binding toan bo du lieu vao form. Copy thi tru Ma thanh phan va Ten thanh phan ra
 */
const initForm = () => {
  errors.value = {};
  isCodeManuallyEdited.value = false;
  if (props.mode === 'edit' || props.mode === 'copy' || props.mode === 'view') {
    if (props.initialData) {
      formData.value = {
        ...getDefaultData(),
        ...props.initialData,
        salaryComponentSystemId: props.initialData.salaryComponentSystemId,
        appliedUnitIds: props.initialData.appliedUnitIds || []
      };
      // neu la che do Nhan ban, thi 2 truong nay can phai trong de nguoi dung tu cau hinh componentCode va componentName moi
      if (props.mode === 'copy') {
        formData.value.componentCode = '';
        formData.value.componentName = '';
      }
      
      // Ensure numeric binding for attribute and valueType if they exist
      if (props.initialData.attribute !== undefined && props.initialData.attribute !== null) {
        formData.value.attribute = Number(props.initialData.attribute);
      }
      if (props.initialData.valueType !== undefined && props.initialData.valueType !== null) {
        formData.value.valueType = Number(props.initialData.valueType);
      }

      if (props.initialData.value) {
        formData.value.valueFormula = props.initialData.value;
      }

      // Sync tree selection
      if (formData.value.appliedUnitIds && formData.value.appliedUnitIds.length > 0) {
        treeBoxValue.value = [...formData.value.appliedUnitIds];
      } else {
        treeBoxValue.value = [];
      }

    }
  } else {
    formData.value = getDefaultData();
  }
};

const appliedUnits = ref<any[]>([]);
const salaryComponentSystems = ref<any[]>([]);

// Tree selection state
const treeBoxValue = ref<string[]>([]);
const isTreeOpened = ref(false);

const treeDataSource = computed(() => {
  return appliedUnits.value.map(unit => ({
    id: unit.organizationId,
    parentId: unit.parentId || null,
    text: unit.organizationName,
    expanded: true
  }));
});

const onTreeViewSelectionChanged = (e: any) => {
  const nodes = e.component.getSelectedNodes();
  treeBoxValue.value = nodes.map((node: any) => node.key);
  // Update formData
  formData.value.appliedUnitIds = [...treeBoxValue.value];
  clearError('appliedFor');
};

const onTreeItemClick = () => {
  // Option: Close on click if single selection, but user wants chips so probably multi-selection
};

const selectedItems = computed(() => {
  return appliedUnits.value
      .filter(u => treeBoxValue.value.includes(u.organizationId))
      // Chỉ giữ lại những node mà cha của nó không được chọn (top-most selected nodes)
      .filter(u => !u.parentId || !treeBoxValue.value.includes(u.parentId))
      .map(u => ({id: u.organizationId, text: u.organizationName}));
});

const removeTag = (id: string) => {
  // Lấy danh sách tất cả các node con/cháu của node đang bị xóa
  const getDescendantIds = (parentId: string): string[] => {
    const children = appliedUnits.value.filter(u => u.parentId === parentId).map(u => u.organizationId);
    let ids = [...children];
    for (const childId of children) {
      ids = [...ids, ...getDescendantIds(childId)];
    }
    return ids;
  };

  const idsToRemove = new Set([id, ...getDescendantIds(id)]);
  treeBoxValue.value = treeBoxValue.value.filter(v => !idsToRemove.has(v));
  formData.value.appliedUnitIds = [...treeBoxValue.value];
  validateField('appliedFor');
};

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
}, {deep: true});

watch(() => props.mode, () => {
  initForm();
  nextTick(() => componentNameRef.value?.focus());
});

// Ban su kien luu ban ghi len component cha
const handleSave = () => {
  if (!validateAll()) {
    focusFirstError();
    return;
  }
  emit('save', formData.value, false); // stayOpen = false
  emit('close');
  formData.value = getDefaultData();
  console.log(formData.value);
};

const handleSaveAndAdd = async () => {
  if (!validateAll()) {
    focusFirstError();
    return;
  }
  emit('save', formData.value, true); // stayOpen = true
  // Reset form cho lần thêm tiếp theo
  formData.value = getDefaultData();
  errors.value = {};
  // nextTick(() => componentNameRef.value?.focus());
  await nextTick(() => componentNameRef.value?.focus());
};
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
          <div class="salary-form-title">{{ mode === 'edit' ? 'Sửa' : mode === 'view' ? 'Chi tiết' : 'Thêm' }} thành phần</div>
        </div>
        <div class="salary-form-header-right">
          <button class="misa-btn-cancel" @click="$emit('close')">Hủy bỏ</button>
          <template v-if="mode !== 'view'">
            <button class="misa-btn-outline" @click="handleSaveAndAdd">Lưu và thêm</button>
            <button class="misa-btn-primary" @click="handleSave">Lưu</button>
          </template>
        </div>
      </div>

      <!-- Body -->
      <div class="salary-form-body">
        <div class="form-wrapper">
          <!-- Tên thành phần -->
          <div class="form-row" :class="{ 'has-error': errors.componentName }">
            <div class="form-label">Tên thành phần <span class="required">*</span></div>
            <div class="form-control">
              <input ref="componentNameRef"
                     type="text"
                     class="misa-input w-full-input"
                     :class="{ 'input-error': errors.componentName }"
                     v-model="formData.componentName"
                     @input="onComponentNameInput"
                     @blur="validateField('componentName')"
                     maxlength="255"
                     :disabled="mode === 'view'"
                     tabindex="1"/>
              <div v-if="errors.componentName" class="error-message">{{ errors.componentName }}</div>
            </div>
          </div>

          <!-- Mã thành phần -->
          <div class="form-row" :class="{ 'has-error': errors.componentCode }">
            <div class="form-label">Mã thành phần <span class="required">*</span></div>
            <div class="form-control">
              <input ref="componentCodeRef"
                     type="text"
                     class="misa-input w-full-input"
                     :class="{ 'input-error': errors.componentCode }"
                     v-model="formData.componentCode"
                     placeholder="Nhập mã viết liền"
                     @input="onComponentCodeInput"
                     @blur="validateField('componentCode')"
                     maxlength="255"
                     :disabled="mode === 'view'"
                     tabindex="2"/>
              <div v-if="errors.componentCode" class="error-message">{{ errors.componentCode }}</div>
            </div>
          </div>

          <!-- Đơn vị áp dụng -->
          <div class="form-row" :class="{ 'has-error': errors.appliedFor }">
            <div class="form-label">Đơn vị áp dụng <span class="required">*</span></div>
            <div class="form-control">
              <DxDropDownBox
                  class="misa-selectbox w-full-input unit-dropdown"
                  :class="{ 'select-error': errors.appliedFor }"
                  v-model:value="treeBoxValue"
                  v-model:opened="isTreeOpened"
                  :data-source="treeDataSource"
                  value-expr="id"
                  display-expr="text"
                  placeholder="--- Tất cả đơn vị ---"
                  content-template="tree-template"
                  field-template="field-template"
                  :drop-down-options="{ container: '.salary-form-container', wrapperAttr: { class: 'misa-dropdown-tree-popup' } }"
                  :tab-index="3"
                  :disabled="mode === 'view'"
              >
                <template #field-template="{ data }">
                  <div class="misa-tagbox-field">
                    <div class="misa-tagbox-tags">
                      <div v-for="item in selectedItems" :key="item.id" class="misa-tag">
                        <span class="misa-tag-text">{{ item.text }}</span>
                        <span class="misa-tag-remove" @click.stop="removeTag(item.id)">
                          <svg viewBox="0 0 24 24" width="12" height="12" fill="none" stroke="currentColor"
                               stroke-width="2">
                            <line x1="18" y1="6" x2="6" y2="18"></line>
                            <line x1="6" y1="6" x2="18" y2="18"></line>
                          </svg>
                        </span>
                      </div>
                      <input
                          class="dx-texteditor-input"
                          :class="{ 'hide-value-text': selectedItems.length > 0 }"
                          readonly
                          :placeholder="selectedItems.length === 0 ? '--- Tất cả đơn vị ---' : ''"
                      />
                    </div>
                  </div>
                </template>
                <template #tree-template>
                  <DxTreeView
                      :data-source="treeDataSource"
                      data-structure="plain"
                      key-expr="id"
                      parent-id-expr="parentId"
                      display-expr="text"
                      :select-by-click="true"
                      :select-nodes-recursive="true"
                      show-check-boxes-mode="selectAll"
                      selection-mode="multiple"
                      :selected-item-keys="treeBoxValue"
                      @selection-changed="onTreeViewSelectionChanged"
                      @item-click="onTreeItemClick"
                  />
                </template>
              </DxDropDownBox>
              <div v-if="errors.appliedFor" class="error-message">{{ errors.appliedFor }}</div>
            </div>
          </div>

          <!-- Loại thành phần -->
          <div class="form-row" :class="{ 'has-error': errors.salaryComponentSystemId }">
            <div class="form-label">Loại thành phần <span class="required">*</span></div>
            <div class="form-control" data-field="salaryComponentSystemId">
              <DxSelectBox
                  class="misa-selectbox w-full-input"
                  :class="{ 'select-error': errors.salaryComponentSystemId }"
                  :items="salaryComponentSystems"
                  display-expr="salaryComponentSystemName"
                  value-expr="salaryComponentSystemId"
                  v-model:value="formData.salaryComponentSystemId"
                  @value-changed="clearError('salaryComponentSystemId')"
                  :tab-index="4"
                  :disabled="mode === 'view'"/>
              <div v-if="errors.salaryComponentSystemId" class="error-message">{{ errors.salaryComponentSystemId }}
              </div>
            </div>
          </div>

          <!-- Tính chất -->
          <div class="form-row" :class="{ 'has-error': errors.attribute }">
            <div class="form-label">Tính chất <span class="required">*</span></div>
            <div class="form-control flex-row" data-field="attribute">
              <DxSelectBox
                  class="misa-selectbox"
                  :class="{ 'select-error': errors.attribute }"
                  style="width: 250px"
                  :items="attributeOptions"
                  display-expr="name" value-expr="id"
                  v-model:value="formData.attribute"
                  @value-changed="clearError('attribute')"
                  :tab-index="5"
                  :disabled="mode === 'view'"/>

              <!-- Radio thuế: chỉ hiển thị khi Tính chất = Thu nhập -->
              <div v-if="showTaxOptions && formData.attribute === 1" class="radio-group ml-24">
                <label v-for="tax in taxOptions" :key="tax.value" class="radio-label" :class="{'disabled-label': mode === 'view'}">
                  <input type="radio" v-model="formData.taxType" :value="tax.value" :tabindex="tax.tabindex" :disabled="mode === 'view'">
                  <span class="radio-custom"></span> {{ tax.label }}
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
                <MsFormula
                    ref="quotaFormulaRef"
                    v-model="formData.quota"
                    placeholder="Tự động gợi ý công thức và tham số khi gõ"
                    :rows="6"
                    :disabled="mode === 'view'"/>
              </div>
              <div class="checkbox-container mt-16">
                <label class="checkbox-label" :class="{'disabled-label': mode === 'view'}">
                  <input type="checkbox" v-model="formData.allowExceedQuota" tabindex="10" :disabled="mode === 'view'">
                  <span class="checkbox-custom"></span> Cho phép giá trị tính vượt quá định mức
                </label>
                <div class="info-icon-container ml-4" title="Giải thích về định mức">
                  <InfoIcon/>
                </div>
              </div>
            </div>
          </div>

          <!-- Kiểu giá trị -->
          <div class="form-row">
            <div class="form-label">Kiểu giá trị</div>
            <div class="form-control">
              <DxSelectBox
                  class="misa-selectbox misa-selectbox-gray"
                  style="width: 250px"
                  :items="valueTypeOptions"
                  display-expr="name"
                  value-expr="id"
                  v-model:value="formData.valueType"
                  :tab-index="11"
                  :disabled="mode === 'view'"/>
            </div>
          </div>

          <!-- Giá trị -->
          <div class="form-row align-top">
            <div class="form-label pt-8">Giá trị</div>
            <div class="form-control flex-col">
              <div class="radio-row mb-12">
                <label class="radio-label" :class="{'disabled-label': mode === 'view'}">
                  <input type="radio" v-model="formData.valueCalculation" value="Tự động cộng tổng" tabindex="12" :disabled="mode === 'view'">
                  <span class="radio-custom"></span> Tự động cộng tổng giá trị của các nhân viên
                </label>
                <div class="inline-selectbox-wrapper ml-12">
                  <DxSelectBox
                      class="misa-selectbox misa-selectbox-gray"
                      style="width: 250px"
                      :items="calculationTargetOptions"
                      v-model="formData.valueCalculationTarget"
                      :disabled="mode === 'view' || formData.valueCalculation !== 'Tự động cộng tổng'"
                      :tab-index="13">
                    <DxButton name="info" location="after" :options="{
                      icon: 'info',
                      type: 'default',
                      stylingMode: 'text',
                      elementAttr: { class: 'info-button-inside' }
                    }"/>
                    <DxButton name="dropDown"/>
                  </DxSelectBox>
                </div>
              </div>

              <div class="radio-row mb-12">
                <label class="radio-label" :class="{'disabled-label': mode === 'view'}">
                  <input
                      type="radio"
                      v-model="formData.valueCalculation"
                      value="Tính theo công thức tự đặt"
                      tabindex="14"
                      :disabled="mode === 'view'">
                  <span class="radio-custom"></span> Tính theo công thức tự đặt
                </label>
              </div>

              <div class="w-full-input">
                <MsFormula
                    ref="valueFormulaRef"
                    v-model="formData.valueFormula"
                    placeholder="Tự động gợi ý công thức và tham số khi gõ"
                    :disabled="mode === 'view' || formData.valueCalculation !== 'Tính theo công thức tự đặt'"
                    :rows="6"/>
              </div>
            </div>
          </div>

          <!-- Mô tả -->
          <div class="form-row align-top mt-24">
            <div class="form-label pt-8">Mô tả</div>
            <div class="form-control">
              <textarea
                  class="misa-textarea w-full-input"
                  rows="3"
                  v-model="formData.description"
                  tabindex="15"
                  :disabled="mode === 'view'"/>
            </div>
          </div>

          <!-- Hiển thị trên phiếu lương -->
          <div class="form-row mt-24">
            <div class="form-label">Hiển thị trên phiếu lương</div>
            <div class="form-control">
              <div class="radio-group">
                <label v-for="option in showOnPayslipOptions" :key="option.value" class="radio-label" :class="{'disabled-label': mode === 'view'}">
                  <input type="radio" v-model="formData.showOnPayslip" :value="option.value"
                         :tabindex="option.tabindex" :disabled="mode === 'view'">
                  <span class="radio-custom"></span> {{ option.label }}
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

<style scoped src="./style.css"></style>

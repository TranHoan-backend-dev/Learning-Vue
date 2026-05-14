<script setup lang="ts">
import { DxPopup, DxToolbarItem } from 'devextreme-vue/popup';
import DxCheckBox from 'devextreme-vue/check-box';
import { VueDraggableNext as draggable } from 'vue-draggable-next';
import MSIcon from "@/components/ui/ms-icon/MSIcon.vue";

interface Props {
  isConfirmVisible: boolean;
  selectedComposition: any;
  isConfigVisible: boolean;
  columns: any[];
  isDeleteVisible: boolean;
  deleteMessage: string;
}

defineProps<Props>();

const emit = defineEmits([
  'update:isConfirmVisible',
  'update:isConfigVisible',
  'update:columns',
  'confirmActive',
  'closeConfirm',
  'closeConfig',
  'update:isDeleteVisible',
  'confirmDelete'
]);

const closeConfirmModal = () => {
  emit('update:isConfirmVisible', false);
  emit('closeConfirm');
};

const confirmActive = () => {
  emit('confirmActive');
};

const closeConfig = () => {
  emit('update:isConfigVisible', false);
  emit('closeConfig');
};

const onColumnsUpdate = (val: any[]) => {
  emit('update:columns', val);
};
</script>

<template>
  <div class="salary-composition-popups">
    <!-- Confirm Modal using DevExtreme DxPopup -->
    <DxPopup
        :visible="isConfirmVisible"
        @update:visible="val => emit('update:isConfirmVisible', val)"
        :width="480"
        height="auto"
        title="Chuyển trạng thái"
        :show-close-button="true"
        :drag-enabled="false"
    >
      <div class="misa-popup-body">
        Bạn có chắc chắn muốn chuyển trạng thái thành phần lương <strong>{{ selectedComposition?.componentName }}</strong>
        sang đang theo dõi không?
      </div>

      <DxToolbarItem toolbar="bottom" location="after" template="cancelBtn"/>
      <DxToolbarItem toolbar="bottom" location="after" template="confirmBtn"/>

      <template #cancelBtn>
        <button class="misa-btn-cancel" @click="closeConfirmModal">Hủy bỏ</button>
      </template>

      <template #confirmBtn>
        <button class="misa-btn-primary" @click="confirmActive">Đồng ý</button>
      </template>
    </DxPopup>

    <!-- Column Configuration Popup -->
    <DxPopup
        :visible="isConfigVisible"
        @update:visible="val => emit('update:isConfigVisible', val)"
        :width="450"
        :height="500"
        title="Cấu hình cột"
        :show-close-button="true"
        :drag-enabled="true"
    >
      <div class="config-popup-content">
        <div class="config-search">
          <input type="text" class="misa-search-input" placeholder="Tìm kiếm" style="width: 100%;"/>
        </div>
        <div class="column-list-wrapper">
          <draggable
              :model-value="columns"
              @update:model-value="onColumnsUpdate"
              handle=".drag-handle"
              item-key="dataField"
          >
            <div v-for="col in columns" :key="col.dataField" class="column-item">
              <div class="column-item-left">
                <div class="drag-handle">
                  <MSIcon name="drag-handle" size="16" color="#888" />
                </div>
                <DxCheckBox v-model="col.visible"/>
                <span class="column-caption">{{ col.caption }}</span>
              </div>
            </div>
          </draggable>
        </div>
      </div>
      <DxToolbarItem toolbar="bottom" location="after" template="configCancelBtn"/>
      <DxToolbarItem toolbar="bottom" location="after" template="configSaveBtn"/>

      <template #configCancelBtn>
        <button class="misa-btn-cancel" @click="closeConfig">Hủy bỏ</button>
      </template>
      <template #configSaveBtn>
        <button class="misa-btn-primary" @click="closeConfig">Lưu</button>
      </template>
    </DxPopup>

    <!-- Delete Confirmation Modal -->
    <DxPopup
        :visible="isDeleteVisible"
        @update:visible="val => emit('update:isDeleteVisible', val)"
        :width="444"
        height="auto"
        title="Thông báo"
        :show-close-button="true"
        :drag-enabled="false"
    >
      <div class="misa-popup-body">
        {{ deleteMessage }}
      </div>

      <DxToolbarItem toolbar="bottom" location="after" template="deleteCancelBtn"/>
      <DxToolbarItem toolbar="bottom" location="after" template="deleteConfirmBtn"/>

      <template #deleteCancelBtn>
        <button class="misa-btn-cancel" @click="emit('update:isDeleteVisible', false)">Hủy</button>
      </template>

      <template #deleteConfirmBtn>
        <button class="misa-btn-danger" @click="emit('confirmDelete')">Xóa</button>
      </template>
    </DxPopup>
  </div>
</template>

<style scoped>
.misa-popup-body {
  font-size: 15px;
  color: var(--misa-text-body);
  line-height: 1.5;
  padding: 8px 0;
}

.misa-btn-cancel {
  padding: var(--misa-padding-button);
  border: 1px solid var(--misa-border-color);
  background: var(--misa-white);
  border-radius: var(--misa-border-radius);
  color: var(--misa-text-primary);
  font-weight: 600;
  cursor: pointer;
  font-family: inherit;
  font-size: var(--misa-font-size-base);
  transition: var(--misa-transition);
}

.misa-btn-cancel:hover {
  background: #f5f5f5;
}

.misa-btn-primary {
  padding: var(--misa-padding-button);
  border: 1px solid transparent;
  background: var(--primary-green);
  border-radius: var(--misa-border-radius);
  color: var(--misa-white);
  font-weight: 600;
  cursor: pointer;
  font-family: inherit;
  font-size: var(--misa-font-size-base);
  transition: var(--misa-transition);
}

.misa-btn-primary:hover {
  background: var(--primary-green-hover);
}

.misa-btn-danger {
  padding: var(--misa-padding-button);
  border: 1px solid transparent;
  background: var(--misa-danger);
  border-radius: var(--misa-border-radius);
  color: var(--misa-white);
  font-weight: 600;
  cursor: pointer;
  font-family: inherit;
  font-size: var(--misa-font-size-base);
  transition: var(--misa-transition);
}

.misa-btn-danger:hover {
  background: var(--misa-danger-hover);
}

.config-popup-content {
  display: flex;
  flex-direction: column;
  gap: 16px;
  height: 100%;
}

.config-search {
  flex-shrink: 0;
}

.column-list-wrapper {
  flex: 1;
  overflow-y: auto;
  border: 1px solid var(--misa-border-color);
  border-radius: var(--misa-border-radius);
  padding: 8px 0;
}

.column-item {
  display: flex;
  align-items: center;
  padding: 8px 12px;
  background: var(--misa-white);
  border-bottom: 1px solid #f0f0f0;
  transition: background-color 0.2s;
}

.column-item:hover {
  background-color: #f9f9f9;
}

.column-item-left {
  display: flex;
  align-items: center;
  gap: 12px;
}

.drag-handle {
  cursor: grab;
  display: flex;
  align-items: center;
}

.drag-handle:active {
  cursor: grabbing;
}

.column-caption {
  font-size: var(--misa-font-size-base);
  color: var(--misa-text-body);
}

.misa-search-input {
  height: var(--misa-control-height-small);
  padding: 0 12px 0 32px;
  border: 1px solid #e0e0e0;
  border-radius: var(--misa-border-radius);
  font-family: inherit;
  font-size: var(--misa-font-size-small);
  color: var(--misa-text-primary);
  outline: none;
  background-color: var(--misa-white);
  transition: border-color 0.2s;
  background-image: url("data:image/svg+xml;charset=utf-8,%3Csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' fill='none' stroke='%23888' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'%3E%3Ccircle cx='11' cy='11' r='6'%3E%3C/circle%3E%3Cline x1='20' y1='20' x2='15.24' y2='15.24'%3E%3C/line%3E%3C/svg%3E");
  background-repeat: no-repeat;
  background-position: 8px center;
  background-size: 16px 16px;
}

.misa-search-input:hover,
.misa-search-input:focus {
  border-color: var(--primary-green);
}
</style>

<script setup lang="ts">
import {watch} from 'vue';
import NormalInput from "@/components/ui/ms-input/NormalInput.vue";
import {validateEmail, validateFullName, validatePhone} from "@/utils/validation.ts"
import {error, form} from "@/views/ms-candidate/add-edit-form/declaration.ts";
import CustomButton from "@/components/ui/ms-button/CustomButton.vue";
import DateInput from "@/components/ui/ms-input/DateInput.vue";
import CustomSelect from "@/components/ui/ms-select/CustomSelect.vue";
import CustomTextarea from "@/components/ui/ms-input/CustomTextarea.vue";

const props = defineProps<{
  modelValue: any;
  mode: 'add' | 'edit';
}>();

const emit = defineEmits(['update:modelValue', 'save', 'cancel']);

watch(() => props.modelValue, (val) => {
  if (val) {
    form.value = {...form.value, ...val};
  } else {
    // Reset form
    form.value = {
      name: '',
      phone: '',
      email: '',
      dob: '',
      gender: '',
      area: '',
      address: '',
      educationLevel: '',
      educationLocation: '',
      major: '',
      hiring_campaign: '',
      hiring_position: '',
      hiring_at: new Date().toISOString().substr(0, 10),
      candidateSource: '',
      recruiter: 'Đình Nga QTHT',
      collaborator: '',
      recentWorkplace: '',
      workplace: '',
      startDate: '',
      endDate: '',
      jobPosition: '',
      jobDescription: '',
      review: '',
    };
  }
}, {immediate: true});

const save = () => {
  emit('save', form.value);
  console.log(form.value);
};

watch(() => form.value.name, (val) => {
  if (!val) {
    error.value.name = 'Ten khong duoc de trong'
  }
  try {
    validateFullName(form.value.name)
    error.value.name = ''
  } catch (e: any) {
    error.value.name = e.message
  }
})

watch(() => form.value.phone, (val) => {
  if (!val) {
    error.value.name = 'So dien thoai khong duoc de trong'
  }
  try {
    validatePhone(form.value.phone)
    error.value.phone = ''
  } catch (e: any) {
    error.value.phone = e.message
  }
})

watch(() => form.value.email, (val) => {
  if (!val) {
    error.value.email = 'Email khong duoc de trong'
  }
  try {
    validateEmail(form.value.email)
    error.value.email = ''
  } catch (e: any) {
    error.value.email = e.message
  }
})

watch(() => form.value.hiring_at, (val) => {
  error.value.ngayUngTuyen = !val ? 'Ngay ung tuyen khong duoc de trong' : ''
})

watch(() => form.value.recruiter, (val) => {
  error.value.nhanSuKhaiThac = !val ? 'Nhan su khai thac khong duoc de trong' : ''
})

watch(() => form.value.startDate, (val) => {
  error.value.thoiGianBatDau = !val ? 'Thoi gian bat dau khong duoc de trong' : ''
})
</script>

<template>
  <div class="modal__body">
    <!-- Upload CV -->
    <div class="upload__container">
      <div class="upload__box">
        <div class="upload__icon">
          <i class="bi bi-cloud-arrow-up"></i>
        </div>
        <p class="upload__text">Kéo thả hoặc bấm vào đây để tải CV lên</p>
        <p class="upload__hint">Chấp nhận file .doc, .docx, .pdf, .jpg, .jpeg, .png (Dung lượng &lt; hơn 15 Mb)</p>
      </div>
    </div>

    <!-- Form Fields -->
    <form class="form__container" @submit.prevent="save">
      <div class="form__left">
        <div class="avatar__upload">
          <div class="avatar__placeholder">Ảnh</div>
        </div>
      </div>

      <div class="form__right">
        <!-- Họ tên -->
        <div class="form__row">
          <label>Họ và tên <span class="required">*</span></label>
          <NormalInput
              type="text"
              v-model="form.name"
              placeholder="Nhập họ và tên"
              :error-message="error.name"
          />
        </div>

        <!-- Ngày sinh, Giới tính -->
        <div class="form__row flex__row gap-16">
          <div class="flex__grow">
            <label>Ngày sinh <span class="sub__label">Ngày tháng năm</span></label>
            <div class="input__with__icon">
              <DateInput
                  v-model="form.dob"
                  aria-label="Ngày tháng năm"
              />
            </div>
          </div>
          <div class="flex__grow">
            <CustomSelect
                label="Giới tính"
                v-model="form.gender"
                :options="[
              { label: 'Chọn giới tính', value: '' },
              { label: 'Nam', value: 'Nam' },
              { label: 'Nữ', value: 'Nữ' }
            ]"/>
          </div>
        </div>

        <!-- Khu vực -->
        <div class="form__row">
          <label>Khu vực</label>
          <div class="input__group">
            <CustomSelect
                v-model="form.area"
                class="flex__grow"
                :options="[
                    {label: 'Chọn giá trị', value: ''},
                    {label: 'Ba Đình', value: 'Ba Đình'},
                    {label: 'Hoàn Kiếm', value: 'Hoàn Kiếm'},
                    {label: 'Hai Bà Trưng', value: 'Hai Bà Trưng'},
                ]"
            />
            <CustomButton
                size="x-small"
                type="button"
                class="btn__more"
                variant="text"
            >
              ...
            </CustomButton>
          </div>
        </div>

        <!-- Sdt, email -->
        <div class="form__row flex__row gap-16">
          <div class="flex__grow">
            <label>Số điện thoại <span class="required">*</span></label>
            <NormalInput
                type="text"
                v-model="form.phone"
                placeholder="Nhập số điện thoại"
                :error-message="error.phone"
            />
          </div>
          <div class="flex__grow">
            <label>Email <span class="required">*</span></label>
            <NormalInput
                type="email"
                v-model="form.email"
                placeholder="Nhập Email"
                :error-message="error.email"
            />
          </div>
        </div>

        <!-- Địa chỉ -->
        <div class="form__row">
          <label>Địa chỉ</label>
          <NormalInput
              type="text"
              v-model="form.address"
              placeholder="Nhập địa chỉ"
          />
        </div>

        <div class="section__title">HỌC VẤN</div>

        <div class="education__section">
          <div class="form__row flex__row gap-8 align-center">
            <label class="edu__label__fixed">• Trình độ đào tạo</label>
            <div class="input__with__actions flex__grow">
              <NormalInput
                  type="text"
                  v-model="form.educationLevel"
                  placeholder="Nhập trình độ đào tạo"
              />
              <div class="actions">
                <i class="bi bi-plus"></i>
                <i class="bi bi-chevron-down"></i>
              </div>
            </div>
          </div>
          <div class="form__row flex__row gap-8 align-center">
            <label class="edu__label__fixed">• Nơi đào tạo</label>
            <div class="input__with__actions flex__grow">
              <NormalInput
                  type="text"
                  v-model="form.educationLocation"
                  placeholder="Nhập nơi đào tạo"
              />
              <div class="actions">
                <i class="bi bi-plus"></i>
                <i class="bi bi-chevron-down"></i>
              </div>
            </div>
          </div>
          <div class="form__row flex__row gap-8 align-center">
            <label class="edu__label__fixed">• Chuyên ngành</label>
            <div class="input__with__actions flex__grow">
              <NormalInput
                  type="text"
                  v-model="form.major"
                  placeholder="Nhập chuyên ngành"
              />
              <div class="actions">
                <i class="bi bi-plus"></i>
                <i class="bi bi-chevron-down"></i>
              </div>
            </div>
          </div>
          <a href="#" class="add__education__link">+ Thêm học vấn</a>
        </div>

        <hr class="divider">

        <!-- Ngày ứng tuyển, Nguồn ứng viên -->
        <div class="form__row flex__row gap-16">
          <div class="flex__grow">
            <label>Ngày ứng tuyển <span class="required">*</span></label>
            <div class="input__with__icon">
              <DateInput
                  v-model="form.hiring_at"
                  :error-message="error.ngayUngTuyen"
              />
            </div>
          </div>
          <div class="flex__grow">
            <CustomSelect
                label="Nguồn ứng viên"
                v-model="form.candidateSource"
                :options="[
                    {label: 'Chọn nguồn ứng viên', value: ''}
                ]"
            />
          </div>
        </div>

        <!-- Nhân sự khai thác + cộng tác viên -->
        <div class="form__row flex__row gap-16">
          <div class="flex__grow">
            <CustomSelect
                label="Nhân sự khai thác"
                :required="true"
                v-model="form.recruiter"
                :options="[
                    {label: 'Đình Nga QTHT', value: 'Đình Nga QTHT'}
                ]"
            />
          </div>
          <div class="flex__grow">
            <CustomSelect
                label="Cộng tác viên"
                v-model="form.collaborator"
                :options="[
                    {label: 'Chọn cộng tác viên', value: ''},
                    {label: 'A', value: 'A'},
                    {label: 'B', value: 'B'},
                    {label: 'C', value: 'C'},
                ]"
            />
          </div>
        </div>

        <div class="form__row" style="display: flex; align-items: center; gap: 8px;">
          <input type="checkbox" id="quickAdd" style="width: auto; height: auto;">
          <label for="quickAdd" style="margin-bottom: 0; font-weight: 400;">
            Thêm nhanh người tham chiếu vào kho ứng viên
          </label>
        </div>

        <div class="form__row">
          <a href="#" class="add__education__link">+ Thêm người giới thiệu</a>
          <div style="margin-top: 12px">
            <label>Nơi làm việc gần đây</label>
            <NormalInput placeholder="Nhập nơi làm việc gần đây" v-model="form.recentWorkplace"/>
          </div>
        </div>

        <hr class="divider">

        <div class="form__row">
          <a href="#" class="add__education__link">+ Thêm kinh nghiệm làm việc</a>
          <div style="margin-top: 12px;">
            <label>Nơi làm việc</label>
            <NormalInput placeholder="Nhập nơi làm việc" v-model="form.workplace"/>
          </div>
        </div>

        <div class="form__row flex__row gap-16">
          <div class="flex__grow">
            <label>Thời gian bắt đầu <span class="required">*</span></label>
            <div class="input__with__icon">
              <DateInput v-model="form.startDate"/>
            </div>
          </div>
          <div style="padding-bottom: 10px;">-</div>
          <div class="flex__grow">
            <label>Thời gian kết thúc</label>
            <div class="input__with__icon">
              <DateInput v-model="form.endDate"/>
            </div>
          </div>
        </div>

        <div class="form__row">
          <label>Vị trí công việc</label>
          <NormalInput placeholder="Nhập vị trí công việc" v-model="form.jobPosition"/>
        </div>

        <div class="form__row">
          <label>Mô tả công việc</label>
          <CustomTextarea v-model="form.jobDescription" placeholder="Nhập mô tả công việc"/>
        </div>
      </div>
    </form>
  </div>
  <div class="modal__footer">
    <button type="button" class="btn__cancel" @click="$emit('cancel')">Hủy</button>
    <button type="submit" class="btn__save" @click="save">
      {{ props.mode === 'add' ? 'Lưu' : 'Cập nhật' }}
    </button>
  </div>
</template>

<style scoped src="./style.css">

</style>

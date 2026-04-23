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
    form.value = {
      ...form.value,
      ...val,
      candidateId: val.candidateId || val.id || ''
    };
  } else {
    // Reset form
    form.value = {
      candidateId: '',
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
      hiringCampaign: '',
      hiringPosition: '',
      hiringRound: 'Hồ sơ',
      hiringAt: new Date().toISOString().substr(0, 10),
      status: true,
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

  // Also reset errors
  error.value = {
    name: '',
    phone: '',
    email: '',
    ngayUngTuyen: '',
    nhanSuKhaiThac: '',
    thoiGianBatDau: '',
    dob: ''
  };
}, {immediate: true});

const save = () => {
  if (validateAll()) {
    // Filter payload to only send what the backend needs
    const payload: any = {
      name: form.value.name,
      phone: form.value.phone,
      email: form.value.email,
      hiringCampaign: form.value.hiringCampaign,
      hiringPosition: form.value.hiringPosition,
      hiringRound: form.value.hiringRound,
      hiringAt: form.value.hiringAt,
      status: form.value.status,
      review: form.value.review || '',
    };
    if (form.value.candidateId) {
      payload.candidateId = form.value.candidateId;
    }
    emit('save', payload);
    console.log('Filtered payload:', payload);
  } else {
    console.log('Validation failed', error.value);
  }
};

const validateAll = () => {
  let isValid = true;

  // Validate Name
  if (!form.value.name) {
    error.value.name = 'Tên không được để trống';
    isValid = false;
  } else {
    try {
      validateFullName(form.value.name);
      error.value.name = '';
    } catch (e: any) {
      error.value.name = e.message;
      isValid = false;
    }
  }

  // Validate Phone
  if (!form.value.phone) {
    error.value.phone = 'Số điện thoại không được để trống';
    isValid = false;
  } else {
    try {
      validatePhone(form.value.phone);
      error.value.phone = '';
    } catch (e: any) {
      error.value.phone = e.message;
      isValid = false;
    }
  }

  // Validate Email
  if (!form.value.email) {
    error.value.email = 'Email không được để trống';
    isValid = false;
  } else {
    try {
      validateEmail(form.value.email);
      error.value.email = '';
    } catch (e: any) {
      error.value.email = e.message;
      isValid = false;
    }
  }

  // Validate other required fields
  error.value.hiringCampaign = !form.value.hiringCampaign ? 'Chiến dịch không được để trống' : '';
  if (error.value.hiringCampaign) isValid = false;

  error.value.hiringPosition = !form.value.hiringPosition ? 'Vị trí không được để trống' : '';
  if (error.value.hiringPosition) isValid = false;

  error.value.ngayUngTuyen = !form.value.hiringAt ? 'Ngày ứng tuyển không được để trống' : '';
  if (error.value.ngayUngTuyen) isValid = false;

  error.value.nhanSuKhaiThac = !form.value.recruiter ? 'Nhân sự khai thác không được để trống' : '';
  if (error.value.nhanSuKhaiThac) isValid = false;

  error.value.thoiGianBatDau = !form.value.startDate ? 'Thời gian bắt đầu không được để trống' : '';
  if (!error.value.thoiGianBatDau) {
    const startDate = new Date(form.value.startDate);
    startDate.setHours(0, 0, 0, 0);
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    if (startDate > today) {
      error.value.thoiGianBatDau = 'Thời gian bắt đầu không thể là tương lai';
    }
  }
  if (error.value.thoiGianBatDau) isValid = false;

  // Validate Ngày ứng tuyển (not future)
  if (!error.value.ngayUngTuyen) {
    const hiringAt = new Date(form.value.hiringAt);
    hiringAt.setHours(0, 0, 0, 0);
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    if (hiringAt > today) {
      error.value.ngayUngTuyen = 'Ngày ứng tuyển không thể là tương lai';
      isValid = false;
    }
  }

  // Validate DOB (18+)
  if (form.value.dob) {
    const dob = new Date(form.value.dob);
    const today = new Date();
    let age = today.getFullYear() - dob.getFullYear();
    const m = today.getMonth() - dob.getMonth();
    if (m < 0 || (m === 0 && today.getDate() < dob.getDate())) {
      age--;
    }
    if (age < 18) {
      error.value.dob = 'Ứng viên phải đủ 18 tuổi';
      isValid = false;
    } else {
      error.value.dob = '';
    }
  }

  return isValid;
};

watch(() => form.value.name, (val) => {
  if (!val) {
    error.value.name = 'Tên không được để trống';
    return;
  }
  try {
    validateFullName(val);
    error.value.name = '';
  } catch (e: any) {
    error.value.name = e.message;
  }
});

watch(() => form.value.phone, (val) => {
  if (!val) {
    error.value.phone = 'Số điện thoại không được để trống';
    return;
  }
  try {
    validatePhone(val);
    error.value.phone = '';
  } catch (e: any) {
    error.value.phone = e.message;
  }
});

watch(() => form.value.email, (val) => {
  if (!val) {
    error.value.email = 'Email không được để trống';
    return;
  }
  try {
    validateEmail(val);
    error.value.email = '';
  } catch (e: any) {
    error.value.email = e.message;
  }
});

watch(() => form.value.hiringAt, (val) => {
  if (!val) {
    error.value.ngayUngTuyen = 'Ngày ứng tuyển không được để trống';
  } else {
    const hiringAt = new Date(val);
    hiringAt.setHours(0, 0, 0, 0);
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    if (hiringAt > today) {
      error.value.ngayUngTuyen = 'Ngày ứng tuyển không thể là tương lai';
    } else {
      error.value.ngayUngTuyen = '';
    }
  }
});

watch(() => form.value.recruiter, (val) => {
  error.value.nhanSuKhaiThac = !val ? 'Nhân sự khai thác không được để trống' : '';
});

watch(() => form.value.hiringCampaign, (val) => {
  error.value.hiringCampaign = !val ? 'Chiến dịch không được để trống' : '';
});

watch(() => form.value.hiringPosition, (val) => {
  error.value.hiringPosition = !val ? 'Vị trí không được để trống' : '';
});

watch(() => form.value.startDate, (val) => {
  if (!val) {
    error.value.thoiGianBatDau = 'Thời gian bắt đầu không được để trống';
  } else {
    const startDate = new Date(val);
    startDate.setHours(0, 0, 0, 0);
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    if (startDate > today) {
      error.value.thoiGianBatDau = 'Thời gian bắt đầu không thể là tương lai';
    } else {
      error.value.thoiGianBatDau = '';
    }
  }
});

watch(() => form.value.dob, (val) => {
  if (val) {
    const dob = new Date(val);
    const today = new Date();
    let age = today.getFullYear() - dob.getFullYear();
    const m = today.getMonth() - dob.getMonth();
    if (m < 0 || (m === 0 && today.getDate() < dob.getDate())) {
      age--;
    }
    if (age < 18) {
      error.value.dob = 'Ứng viên phải đủ 18 tuổi';
    } else {
      error.value.dob = '';
    }
  } else {
    error.value.dob = '';
  }
});
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
    <form @submit.prevent="save">
      <div class="form__container">
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
          <div class="form__row flex__row gap-16 col-2">
            <div class="flex__grow">
              <label>Ngày sinh <span class="sub__label">Ngày tháng năm</span></label>
              <div class="input__with__icon">
                <DateInput
                    v-model="form.dob"
                    aria-label="Ngày tháng năm"
                    :error-message="error.dob"
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
          <div class="form__row flex__row gap-16 col-2">
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

          <!-- Chiến dịch và Vị trí tuyển dụng -->
          <div class="form__row flex__row gap-16 col-2">
            <div class="flex__grow">
              <label>Chiến dịch tuyển dụng <span class="required">*</span></label>
              <NormalInput
                  v-model="form.hiringCampaign"
                  placeholder="Nhập chiến dịch tuyển dụng"
                  :error-message="error.hiringCampaign"
              />
            </div>
            <div class="flex__grow">
              <label>Vị trí tuyển dụng <span class="required">*</span></label>
              <NormalInput
                  v-model="form.hiringPosition"
                  placeholder="Nhập vị trí tuyển dụng"
                  :error-message="error.hiringPosition"
              />
            </div>
          </div>

          <!-- Ngày ứng tuyển, Nguồn ứng viên -->
          <div class="form__row flex__row gap-16 col-2">
            <div class="flex__grow">
              <label>Ngày ứng tuyển <span class="required">*</span></label>
              <div class="input__with__icon">
                <DateInput
                    v-model="form.hiringAt"
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
          <div class="form__row flex__row gap-16 col-2">
            <div class="flex__grow">
              <CustomSelect
                  label="Nhân sự khai thác"
                  :required="true"
                  v-model="form.recruiter"
                  :error-message="error.nhanSuKhaiThac"
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

          <div class="form__row flex__row gap-16 col-2" style="align-items: flex-start;">
            <div class="flex__grow">
              <label>Thời gian bắt đầu <span class="required">*</span></label>
              <div class="input__with__icon">
                <DateInput
                    v-model="form.startDate"
                    :error-message="error.thoiGianBatDau"
                />
              </div>
            </div>
            <div style="margin-top: 40px;">-</div>
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
      </div>

      <div class="modal__footer">
        <button type="button" class="btn__cancel" @click="$emit('cancel')">Hủy</button>
        <button type="submit" class="btn__save">{{ props.mode === 'add' ? 'Lưu' : 'Cập nhật' }}</button>
      </div>
    </form>
  </div>
</template>

<style scoped src="./style.css">

</style>

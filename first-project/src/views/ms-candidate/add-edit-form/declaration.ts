import {ref} from "vue";

export const form = ref({
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
});

export const error = ref({
    name: '',
    phone: '',
    email: '',
    ngayUngTuyen: '',
    nhanSuKhaiThac: '',
    thoiGianBatDau: ''
})
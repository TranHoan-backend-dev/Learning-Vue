import {ref} from "vue";

export const form = ref({
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
    hiringRound: 'Hồ sơ', // Default hiring round
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
});

export const error = ref({
    name: '',
    phone: '',
    email: '',
    dob: '',
    hiringCampaign: '',
    hiringPosition: '',
    ngayUngTuyen: '',
    nhanSuKhaiThac: '',
    thoiGianBatDau: ''
})
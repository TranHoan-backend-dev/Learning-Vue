import { createWebHistory, createRouter } from "vue-router";
import SalaryCompositions from "@/views/ms-salary-compositions/SalaryCompositions.vue";

const routes = [
    { path: '/compositions', component: SalaryCompositions },
    { path: '/', redirect: '/compositions' },
]

export const router = createRouter({
    history: createWebHistory(),
    routes
})
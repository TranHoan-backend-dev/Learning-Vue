import {createWebHistory, createRouter} from "vue-router";
import SalaryComponents from "@/views/ms-salary-components/SalaryComponents.vue";

const routes = [
    {path: '/components', component: SalaryComponents},
    {path: '/', redirect: '/candidates'},
]

export const router = createRouter({
    history: createWebHistory(),
    routes
})
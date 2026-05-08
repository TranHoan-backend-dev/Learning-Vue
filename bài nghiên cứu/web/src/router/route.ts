import Candidates from "@/views/ms-candidate/Candidates.vue";
import {createWebHistory, createRouter} from "vue-router";

const routes = [
    {path: '/components', component: Candidates},
    {path: '/', redirect: '/candidates'},
]

export const router = createRouter({
    history: createWebHistory(),
    routes
})
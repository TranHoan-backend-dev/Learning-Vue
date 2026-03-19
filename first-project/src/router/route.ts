import Candidates from "@/views/ms-candidate/Candidates.vue";
import Schedule from "@/views/Schedule.vue";
import Conversation from "@/views/Conversation.vue";
import HiringPosts from "@/views/HiringPosts.vue";
import Job from "@/views/Job.vue";
import Marketing from "@/views/Marketing.vue";
import MediaCampaign from "@/views/MediaCampaign.vue";
import Report from "@/views/Report.vue";
import Setting from "@/views/Setting.vue";
import TalentPool from "@/views/TalentPool.vue";
import UsefulKnowledge from "@/views/UsefulKnowledge.vue";
import {createWebHistory, createRouter} from "vue-router";

const routes = [
    {path: '/candidates', component: Candidates},
    {path: '/schedule', component: Schedule},
    {path: '/conversation', component: Conversation},
    {path: '/hiring', component: HiringPosts},
    {path: '/job', component: Job},
    {path: '/marketing', component: Marketing},
    {path: '/media', component: MediaCampaign},
    {path: '/report', component: Report},
    {path: '/setting', component: Setting},
    {path: '/talent', component: TalentPool},
    {path: '/knowledge', component: UsefulKnowledge},
    {path: '/', redirect: '/candidates'}
]

export const router = createRouter({
    history: createWebHistory(),
    routes
})
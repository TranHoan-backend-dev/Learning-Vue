import {ref} from "vue";

export type ToastType = 'success' | 'info' | 'warning' | 'error'

export type ToastItem = {
    id: number;
    title: string;
    text: string;
    color: ToastType;
    timeout?: number;
}

const snackbars = ref<ToastItem[]>([])
let id = 0

const push = (
    title: string,
    text: string,
    color: ToastType,
    timeout: number
) => {
    snackbars.value.push({
        id: ++id,
        title,
        text,
        color,
        timeout,
    })
}

export const toast = {
    snackbars,
    push,

    success(title: string, text: string, timeout?: number) {
        push(title, text, 'success', timeout ?? 3000)
    },

    error(title: string, text: string, timeout?: number) {
        push(title, text, 'error', timeout ?? 3000)
    },

    warning(title: string, text: string, timeout?: number) {
        push(title, text, 'warning', timeout ?? 3000)
    },

    info(title: string, text: string, timeout ?: number) {
        push(title, text, 'info', timeout ?? 3000)
    }
}
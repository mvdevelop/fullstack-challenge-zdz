import { defineNuxtPlugin } from '#app'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import '@mdi/font/css/materialdesignicons.css'
import 'vuetify/styles'

export default defineNuxtPlugin((nuxtApp) => {
  const vuetify = createVuetify({
    components,
    directives,
    theme: {
      defaultTheme: 'light',
      themes: {
        light: {
          colors: {
            primary: '#1976D2',
            secondary: '#42A5F5',
            success: '#2E7D32',
            warning: '#F57F17',
            error: '#C62828',
            info: '#2196F3',
            background: '#F5F5F5',
            surface: '#FFFFFF',
          },
        },
        dark: {
          colors: {
            primary: '#90CAF9',
            secondary: '#42A5F5',
            success: '#66BB6A',
            warning: '#FFA726',
            error: '#EF5350',
            info: '#64B5F6',
            background: '#121212',
            surface: '#1E1E1E',
          },
        },
      },
    },
    defaults: {
      VCard: { rounded: 'lg', elevation: 1 },
      VBtn: { rounded: 'lg' },
      VTextField: { variant: 'outlined', density: 'comfortable' },
      VSelect: { variant: 'outlined', density: 'comfortable' },
    },
  })

  nuxtApp.vueApp.use(vuetify)
})
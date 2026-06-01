<template>
  <v-app>
    <!-- Header -->
    <v-app-bar flat height="52" class="px-4 pf-header">
      <template #prepend>
        <div class="d-flex align-center">
          <v-icon icon="mdi-wallet" size="20" color="primary" class="mr-2" />
          <span class="font-weight-bold" style="font-size:1.1rem">Controle Financeiro</span>
        </div>
      </template>
      <v-spacer />

      <!-- Theme toggle -->
      <v-btn variant="text" size="small" icon @click="toggleTheme">
        <v-icon :icon="darkIcon" size="18" />
      </v-btn>

      <v-menu>
        <template #activator="{ props }">
          <v-btn v-bind="props" variant="text" rounded="xl" height="36">
            <v-avatar color="primary" size="30">
              <span class="text-caption font-weight-bold text-white">{{ userInitials }}</span>
            </v-avatar>
            <span class="ml-2 text-body-2 font-weight-medium" style="max-width:120px;overflow:hidden;text-overflow:ellipsis;white-space:nowrap">{{ auth.user?.nome }}</span>
            <v-icon icon="mdi-chevron-down" size="14" />
          </v-btn>
        </template>
        <v-list min-width="200">
          <v-list-item>
            <template #prepend>
              <v-avatar color="primary" size="36">
                <span class="text-caption font-weight-bold text-white">{{ userInitials }}</span>
              </v-avatar>
            </template>
            <v-list-item-title class="font-weight-medium">{{ auth.user?.nome }}</v-list-item-title>
            <v-list-item-subtitle>{{ auth.user?.email }}</v-list-item-subtitle>
          </v-list-item>
          <v-divider class="my-1" />
          <v-list-item prepend-icon="mdi-logout" title="Sair" @click="auth.logout()" />
        </v-list>
      </v-menu>
    </v-app-bar>

    <!-- Nav Bar -->
    <div class="pf-nav-bar">
      <NuxtLink
        v-for="item in navItems"
        :key="item.to"
        :to="item.to"
        class="pf-nav-btn"
        :class="{ active: route.path === item.to }"
      >
        <v-icon :icon="item.icon" size="14" />
        {{ item.title }}
      </NuxtLink>
    </div>

    <v-main class="pf-main">
      <div class="pf-main-inner">
        <slot />
      </div>
    </v-main>
  </v-app>
</template>

<script setup lang="ts">
import { useRoute } from 'vue-router'
import { useTheme } from 'vuetify'
const route = useRoute()
const auth = useAuthStore()
const vuetifyTheme = useTheme()

const navItems = [
  { title: 'Dashboard', icon: 'mdi-view-dashboard', to: '/dashboard' },
  { title: 'Transações', icon: 'mdi-swap-horizontal', to: '/transacoes' },
  { title: 'Contas', icon: 'mdi-bank', to: '/contas' },
  { title: 'Categorias', icon: 'mdi-tag-multiple', to: '/categorias' },
  { title: 'Orçamentos', icon: 'mdi-target', to: '/orcamentos' },
  { title: 'Relatórios', icon: 'mdi-chart-bar', to: '/relatorios' },
]

const userInitials = computed(() => {
  const nome = auth.user?.nome ?? ''
  return nome.split(' ').map(n => n[0]).slice(0, 2).join('').toUpperCase()
})

// Theme
const darkIcon = computed(() => vuetifyTheme.global.name.value === 'dark' ? 'mdi-weather-night' : 'mdi-white-balance-sunny')

function toggleTheme() {
  const current = vuetifyTheme.global.name.value
  const next = current === 'dark' ? 'light' : 'dark'
  vuetifyTheme.global.name.value = next
  if (process.client) localStorage.setItem('theme', next)
}

// Restore saved theme on mount
if (process.client) {
  const saved = localStorage.getItem('theme')
  if (saved) vuetifyTheme.global.name.value = saved as 'light' | 'dark'
}
</script>

<style scoped>
.pf-header {
  background: linear-gradient(135deg, rgba(25,118,210,.06) 0%, rgba(25,118,210,.015) 100%);
  border-bottom: 1px solid rgba(0,0,0,.06) !important;
}
.v-theme--dark .pf-header {
  background: linear-gradient(135deg, rgba(100,181,246,.06) 0%, rgba(100,181,246,.015) 100%);
  border-bottom-color: rgba(255,255,255,.06) !important;
}

/* Nav bar sits below the v-app-bar (52px). v-main auto-adds 52px top padding. */
.pf-nav-bar {
  position: fixed;
  top: 52px;
  left: 0;
  right: 0;
  z-index: 10;
  display: flex;
  gap: 4px;
  padding: 8px 16px 10px;
  background: var(--v-theme-surface);
  border-bottom: 1px solid rgba(0,0,0,.06);
}
.v-theme--dark .pf-nav-bar { border-bottom-color: rgba(255,255,255,.06); }

.pf-nav-btn {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 7px 16px;
  border: 1px solid rgba(0,0,0,.12);
  border-radius: 8px;
  background: transparent;
  color: rgba(0,0,0,.5);
  cursor: pointer;
  font-size: .8rem;
  font-weight: 500;
  text-decoration: none;
  transition: all .15s;
  white-space: nowrap;
}
.v-theme--dark .pf-nav-btn { border-color: rgba(255,255,255,.12); color: rgba(255,255,255,.5); }
.pf-nav-btn:hover { border-color: #1976D2; color: #1976D2; }
.v-theme--dark .pf-nav-btn:hover { border-color: #90CAF9; color: #90CAF9; }
.pf-nav-btn.active {
  background: rgba(25,118,210,.1);
  color: #1976D2;
  border-color: rgba(25,118,210,.2);
  font-weight: 600;
}
.v-theme--dark .pf-nav-btn.active {
  background: rgba(144,202,249,.12);
  color: #90CAF9;
  border-color: rgba(144,202,249,.2);
}

/* v-main auto-adds padding-top for the v-app-bar (52px).
   Nav bar is ~54px tall (44px height + padding).
   We need additional padding so content starts below both. */
.pf-main-inner {
  padding-top: 54px;
  padding-left: 16px;
  padding-right: 16px;
  padding-bottom: 16px;
  max-width: 960px;
  margin: 0 auto;
  width: 100%;
}

@media (max-width: 600px) {
  .pf-main-inner {
    padding-left: 12px;
    padding-right: 12px;
  }
}
</style>

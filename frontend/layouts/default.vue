<template>
  <v-app>
    <!-- Header -->
    <v-app-bar flat border="b" height="52" class="px-4">
      <template #prepend>
        <div class="d-flex align-center">
          <v-icon icon="mdi-wallet" size="20" color="primary" class="mr-2" />
          <span class="font-weight-bold" style="font-size:1.1rem">FinanceiroApp</span>
        </div>
      </template>
      <v-spacer />
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
        <v-icon :icon="item.icon" size="16" class="mr-1" />
        {{ item.title }}
      </NuxtLink>
    </div>

    <v-main>
      <slot />
    </v-main>
  </v-app>
</template>

<script setup lang="ts">
import { useRoute } from 'vue-router'
const route = useRoute()
const auth = useAuthStore()

const navItems = [
  { title: 'Dashboard', icon: 'mdi-view-dashboard', to: '/' },
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
</script>

<style scoped>
.pf-nav-bar {
  display: flex; gap: 4px; padding: 8px 16px 10px; flex-wrap: wrap;
  background: var(--v-theme-surface);
  border-bottom: 1px solid rgba(0,0,0,.06);
}
.v-theme--dark .pf-nav-bar { border-bottom-color: rgba(255,255,255,.06); }
.pf-nav-btn {
  display: inline-flex; align-items: center; gap: 4px;
  padding: 6px 14px; border: 1px solid transparent; border-radius: 8px;
  background: transparent; color: rgba(0,0,0,.5); cursor: pointer;
  font-size: .8rem; font-weight: 500; text-decoration: none;
  transition: all .15s; white-space: nowrap;
}
.v-theme--dark .pf-nav-btn { color: rgba(255,255,255,.5); }
.pf-nav-btn:hover { background: rgba(25,118,210,.06); color: #1976D2; }
.v-theme--dark .pf-nav-btn:hover { background: rgba(144,202,249,.08); color: #90CAF9; }
.pf-nav-btn.active {
  background: rgba(25,118,210,.1); color: #1976D2;
  border-color: rgba(25,118,210,.2); font-weight: 600;
}
.v-theme--dark .pf-nav-btn.active {
  background: rgba(144,202,249,.12); color: #90CAF9;
  border-color: rgba(144,202,249,.2);
}
</style>
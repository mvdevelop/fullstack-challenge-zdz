import { defineStore } from 'pinia'

export interface User {
  id: string
  nome: string
  email: string
}

interface AuthState {
  user: User | null
  token: string | null
  loading: boolean
}

export const useAuthStore = defineStore('auth', () => {
  const { get, post } = useApi()

  const state = reactive<AuthState>({
    user: null,
    token: null,
    loading: true,
  })

  const isAuthenticated = computed(() => !!state.token)
  const user = computed(() => state.user)
  const token = computed(() => state.token)

  function getToken(): string | null {
    if (process.client) return localStorage.getItem('auth_token')
    return null
  }

  function setToken(t: string) {
    if (process.client) localStorage.setItem('auth_token', t)
    state.token = t
  }

  function clearToken() {
    if (process.client) {
      localStorage.removeItem('auth_token')
      localStorage.removeItem('auth_user')
    }
    state.token = null
    state.user = null
  }

  async function init() {
    const savedToken = getToken()
    if (savedToken) {
      try {
        const savedUser = localStorage.getItem('auth_user')
        if (savedUser) state.user = JSON.parse(savedUser)
        state.token = savedToken
        const u = await get<User>('/auth/me')
        state.user = u
        if (process.client) localStorage.setItem('auth_user', JSON.stringify(u))
      } catch {
        clearToken()
      }
    }
    state.loading = false
  }

  async function login(email: string, senha: string) {
    const res = await post<{ id: string; nome: string; email: string; token: string }>('/auth/login', { email, senha })
    state.user = { id: res.id, nome: res.nome, email: res.email }
    setToken(res.token)
    if (process.client) localStorage.setItem('auth_user', JSON.stringify(state.user))
    return state.user
  }

  async function register(nome: string, email: string, senha: string) {
    const res = await post<{ id: string; nome: string; email: string; token: string }>('/auth/register', { nome, email, senha })
    state.user = { id: res.id, nome: res.nome, email: res.email }
    setToken(res.token)
    if (process.client) localStorage.setItem('auth_user', JSON.stringify(state.user))
    return state.user
  }

  function logout() {
    clearToken()
    navigateTo('/login')
  }

  return { user, token, isAuthenticated, loading: computed(() => state.loading), init, login, register, logout }
})
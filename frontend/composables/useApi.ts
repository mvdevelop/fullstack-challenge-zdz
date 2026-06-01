export function useApi() {
  const config = useRuntimeConfig()
  const BASE = config.public.apiBase

  function getAuthHeader(): HeadersInit {
    const headers: HeadersInit = { 'Content-Type': 'application/json' }
    if (process.client) {
      const token = localStorage.getItem('auth_token')
      if (token) headers['Authorization'] = `Bearer ${token}`
    }
    return headers
  }

  async function get<T>(path: string, params?: Record<string, string>): Promise<T> {
    const url = new URL(`${BASE}${path}`)
    if (params) {
      Object.entries(params).forEach(([k, v]) => v && url.searchParams.append(k, v))
    }
    const res = await fetch(url.toString(), { headers: getAuthHeader() })
    if (!res.ok) {
      const err = await res.json().catch(() => ({ message: res.statusText }))
      throw new Error(err.message || 'Erro na requisição')
    }
    return res.json()
  }

  async function post<T>(path: string, body: unknown): Promise<T> {
    const res = await fetch(`${BASE}${path}`, {
      method: 'POST',
      headers: getAuthHeader(),
      body: JSON.stringify(body),
    })
    if (!res.ok) {
      const err = await res.json().catch(() => ({ message: res.statusText }))
      throw new Error(err.message || 'Erro ao criar')
    }
    return res.json()
  }

  async function put<T>(path: string, body: unknown): Promise<T> {
    const res = await fetch(`${BASE}${path}`, {
      method: 'PUT',
      headers: getAuthHeader(),
      body: JSON.stringify(body),
    })
    if (!res.ok) {
      const err = await res.json().catch(() => ({ message: res.statusText }))
      throw new Error(err.message || 'Erro ao atualizar')
    }
    return res.json()
  }

  async function del(path: string): Promise<void> {
    const res = await fetch(`${BASE}${path}`, { method: 'DELETE', headers: getAuthHeader() })
    if (!res.ok) {
      const err = await res.json().catch(() => ({ message: res.statusText }))
      throw new Error(err.message || 'Erro ao excluir')
    }
  }

  return { get, post, put, del }
}
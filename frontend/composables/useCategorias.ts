import type { Categoria, CategoriaForm } from '~/types'

export function useCategorias() {
  const { get, post, put, del } = useApi()

  const categorias = ref<Categoria[]>([])
  const loading = ref(false)
  const erro = ref<string | null>(null)

  async function buscar(tipo?: string) {
    loading.value = true
    erro.value = null
    try {
      categorias.value = await get<Categoria[]>('/categorias', tipo ? { tipo } : undefined)
    } catch (e: any) {
      erro.value = e.message
    } finally {
      loading.value = false
    }
  }

  async function criar(form: CategoriaForm) {
    loading.value = true
    try {
      const nova = await post<Categoria>('/categorias', form)
      categorias.value.push(nova)
      return nova
    } catch (e: any) {
      erro.value = e.message
      throw e
    } finally {
      loading.value = false
    }
  }

  async function atualizar(id: string, form: CategoriaForm) {
    loading.value = true
    try {
      const atualizada = await put<Categoria>(`/categorias/${id}`, form)
      const idx = categorias.value.findIndex(c => c.id === id)
      if (idx !== -1) categorias.value[idx] = atualizada
      return atualizada
    } catch (e: any) {
      erro.value = e.message
      throw e
    } finally {
      loading.value = false
    }
  }

  async function excluir(id: string) {
    loading.value = true
    try {
      await del(`/categorias/${id}`)
      categorias.value = categorias.value.filter(c => c.id !== id)
    } catch (e: any) {
      erro.value = e.message
      throw e
    } finally {
      loading.value = false
    }
  }

  return { categorias, loading, erro, buscar, criar, atualizar, excluir }
}

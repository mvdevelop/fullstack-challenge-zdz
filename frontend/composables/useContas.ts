import type { Conta, ContaForm } from '~/types'

export function useContas() {
  const { get, post, put, del } = useApi()

  const contas = ref<Conta[]>([])
  const loading = ref(false)
  const erro = ref<string | null>(null)

  async function buscar() {
    loading.value = true
    erro.value = null
    try {
      contas.value = await get<Conta[]>('/contas')
    } catch (e: any) {
      erro.value = e.message
    } finally {
      loading.value = false
    }
  }

  async function criar(form: ContaForm) {
    loading.value = true
    try {
      const nova = await post<Conta>('/contas', form)
      contas.value.push(nova)
      return nova
    } catch (e: any) {
      erro.value = e.message
      throw e
    } finally {
      loading.value = false
    }
  }

  async function atualizar(id: string, form: ContaForm) {
    loading.value = true
    try {
      const atualizada = await put<Conta>(`/contas/${id}`, form)
      const idx = contas.value.findIndex(c => c.id === id)
      if (idx !== -1) contas.value[idx] = atualizada
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
      await del(`/contas/${id}`)
      contas.value = contas.value.filter(c => c.id !== id)
    } catch (e: any) {
      erro.value = e.message
      throw e
    } finally {
      loading.value = false
    }
  }

  const saldoTotal = computed(() =>
    contas.value.filter(c => c.ativa).reduce((acc, c) => acc + c.saldoAtual, 0)
  )

  return { contas, loading, erro, saldoTotal, buscar, criar, atualizar, excluir }
}

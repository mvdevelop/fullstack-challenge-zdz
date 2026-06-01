<template>
  <div>
    <!-- Header: título + saldo total à direita -->
    <div class="pf-page-header">
      <v-icon icon="mdi-credit-card" size="20" color="primary" />
      <h5>Contas</h5>
      <div class="ms-auto text-right">
        <div class="text-caption text-medium-emphasis font-weight-medium">Saldo total</div>
        <div class="text-h5 font-weight-bold text-pf-green">{{ formatarValor(saldoTotal) }}</div>
      </div>
    </div>

    <!-- Lista de contas -->
    <div class="d-flex flex-column ga-3">
      <div v-for="conta in contas" :key="conta.id" class="pf-card pf-account-card" :opacity="conta.ativa ? 1 : 0.5" style="cursor:pointer" @click="abrirDialog(conta)">
        <div class="pf-account-icon" :class="corConta(conta.tipo)">
          <v-icon :icon="iconeTipo(conta.tipo)" size="22" />
        </div>
        <div class="flex-1 min-width-0">
          <div class="font-weight-medium" style="font-size:.9rem">{{ conta.nome }}</div>
          <div class="text-caption text-medium-emphasis">{{ conta.tipo }} · {{ conta.moeda }}</div>
        </div>
        <div class="text-right">
          <div class="font-weight-bold" style="font-size:1.1rem" :class="conta.saldoAtual >= 0 ? 'text-pf-green' : 'text-pf-red'">
            {{ formatarValor(conta.saldoAtual) }}
          </div>
          <span class="pf-badge" :class="badgeStatus(conta)">{{ textoStatus(conta) }}</span>
        </div>
      </div>
    </div>

    <!-- Dialog criar/editar -->
    <v-dialog v-model="dialog" max-width="440" persistent>
      <v-card rounded="lg">
        <v-card-title class="pa-6 pb-2">{{ editando ? 'Editar conta' : 'Nova conta' }}</v-card-title>
        <v-card-text class="pa-6 pt-3">
          <v-text-field v-model="form.nome" label="Nome da conta" class="mb-3" />
          <v-select
            v-model="form.tipo"
            label="Tipo"
            :items="['Corrente', 'Poupanca', 'Cartao', 'Investimento', 'Outro']"
            class="mb-3"
          />
          <v-text-field
            v-model.number="form.saldoInicial"
            label="Saldo inicial"
            prefix="R$"
            type="number"
            :disabled="!!editando"
            class="mb-3"
          />
          <v-select v-model="form.moeda" label="Moeda" :items="['BRL', 'USD', 'EUR']" />
        </v-card-text>
        <v-card-actions class="pa-6 pt-0">
          <v-spacer />
          <v-btn variant="text" @click="dialog = false">Cancelar</v-btn>
          <v-btn color="primary" variant="flat" :loading="loading" @click="salvar">
            {{ editando ? 'Salvar' : 'Criar' }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup lang="ts">
import type { Conta, ContaForm } from '~/types'

definePageMeta({ layout: 'default', middleware: 'auth' })

const { contas, loading, saldoTotal, buscar, criar, atualizar, excluir } = useContas()

const dialog = ref(false)
const editando = ref<Conta | null>(null)
const form = reactive<ContaForm>({ nome: '', tipo: 'Corrente', saldoInicial: 0, moeda: 'BRL' })

function formatarValor(v: number) {
  return `R$ ${v.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}`
}

function corConta(tipo: string) {
  return { Corrente: 'accent', Poupanca: 'green', Cartao: 'red', Investimento: 'accent', Outro: 'accent' }[tipo] ?? 'accent'
}

function iconeTipo(tipo: string) {
  return { Corrente: 'mdi-bank', Poupanca: 'mdi-piggy-bank', Cartao: 'mdi-credit-card', Investimento: 'mdi-chart-line', Outro: 'mdi-wallet' }[tipo] ?? 'mdi-wallet'
}

function badgeStatus(conta: Conta) {
  if (!conta.ativa) return 'red'
  if (conta.tipo === 'Cartao') return 'amber'
  return 'green'
}

function textoStatus(conta: Conta) {
  if (!conta.ativa) return 'Inativa'
  if (conta.tipo === 'Cartao') return 'Fatura aberta'
  return 'Ativa'
}

function abrirDialog(conta?: Conta) {
  editando.value = conta ?? null
  if (conta) Object.assign(form, { nome: conta.nome, tipo: conta.tipo, saldoInicial: conta.saldoInicial, moeda: conta.moeda })
  else Object.assign(form, { nome: '', tipo: 'Corrente', saldoInicial: 0, moeda: 'BRL' })
  dialog.value = true
}

async function salvar() {
  if (editando.value) await atualizar(editando.value.id, form)
  else await criar(form)
  dialog.value = false
}

onMounted(buscar)
</script>

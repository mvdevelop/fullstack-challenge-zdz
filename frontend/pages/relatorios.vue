<template>
  <div>
    <v-row>
      <!-- Resumo Mensal -->
      <v-col cols="12">
        <v-card rounded="lg">
          <v-card-title class="d-flex align-center justify-space-between pa-5">
            <span class="font-weight-semibold">Resumo Mensal</span>
            <v-select
              v-model="mesesSelecionados"
              :items="[3, 6, 12]"
              label="Período"
              hide-details
              density="compact"
              variant="outlined"
              style="max-width: 150px"
              @update:model-value="buscarMensal"
            />
          </v-card-title>
          <v-card-text class="pa-0">
            <v-table hover>
              <thead>
                <tr>
                  <th class="text-left font-weight-semibold">Mês</th>
                  <th class="text-right">Receitas</th>
                  <th class="text-right">Despesas</th>
                  <th class="text-right">Saldo</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="item in resumoMensal" :key="item.mes">
                  <td class="font-weight-medium">{{ formatarMes(item.mes) }}</td>
                  <td class="text-right text-success">{{ formatarValor(item.totalReceitas) }}</td>
                  <td class="text-right text-error">{{ formatarValor(item.totalDespesas) }}</td>
                  <td class="text-right" :class="item.saldo >= 0 ? 'text-success' : 'text-error'">
                    <strong>{{ formatarValor(item.saldo) }}</strong>
                  </td>
                </tr>
                <tr v-if="resumoMensal.length === 0">
                  <td colspan="4" class="text-center text-medium-emphasis py-4">Nenhum dado disponível</td>
                </tr>
              </tbody>
            </v-table>
          </v-card-text>
        </v-card>
      </v-col>

      <!-- Gastos por Categoria -->
      <v-col cols="12">
        <v-card rounded="lg">
          <v-card-title class="d-flex align-center justify-space-between pa-5">
            <span class="font-weight-semibold">Gastos por Categoria</span>
            <v-text-field
              v-model="mesRelatorio"
              label="Mês"
              type="month"
              hide-details
              density="compact"
              variant="outlined"
              style="max-width: 180px"
              @update:model-value="buscarPorCategoria"
            />
          </v-card-title>
          <v-card-text class="pa-5 pt-2">
            <div v-for="item in gastosPorCategoria" :key="item.categoriaId" class="mb-4">
              <div class="d-flex justify-space-between align-center mb-1">
                <div class="d-flex align-center gap-2">
                  <span
                    class="d-inline-block"
                    style="width:10px;height:10px;border-radius:50%"
                    :style="{ background: item.categoriaCor }"
                  />
                  <span class="text-body-2 font-weight-medium">{{ item.categoriaNome }}</span>
                </div>
                <span class="font-weight-semibold">{{ formatarValor(item.total) }}</span>
              </div>
              <v-progress-linear
                :model-value="item.percentual"
                :color="item.categoriaCor"
                rounded
                height="6"
              />
              <div class="text-caption text-medium-emphasis mt-1">{{ Math.round(item.percentual) }}% do total</div>
            </div>
            <div v-if="gastosPorCategoria.length === 0" class="text-center text-medium-emphasis py-4">
              Nenhuma despesa neste período
            </div>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </div>
</template>

<script setup lang="ts">
import type { ResumoMensal, GastoPorCategoria } from '~/types'

definePageMeta({ layout: 'default', middleware: 'auth' })

const { get } = useApi()

const mesesSelecionados = ref(6)
const mesRelatorio = ref(new Date().toISOString().slice(0, 7))
const resumoMensal = ref<ResumoMensal[]>([])
const gastosPorCategoria = ref<GastoPorCategoria[]>([])

function formatarValor(v: number) {
  return `R$ ${v.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}`
}

function formatarMes(mes: string) {
  const [ano, m] = mes.split('-')
  const data = new Date(parseInt(ano), parseInt(m) - 1, 1)
  return data.toLocaleDateString('pt-BR', { month: 'long', year: 'numeric' })
}

async function buscarMensal() {
  resumoMensal.value = await get<ResumoMensal[]>('/relatorios/mensal', { meses: String(mesesSelecionados.value) })
}

async function buscarPorCategoria() {
  gastosPorCategoria.value = await get<GastoPorCategoria[]>('/relatorios/por-categoria', { mes: mesRelatorio.value })
}

onMounted(async () => {
  await Promise.all([buscarMensal(), buscarPorCategoria()])
})
</script>

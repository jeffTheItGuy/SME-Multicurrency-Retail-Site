const API_BASE = '/api';

async function request<T>(path: string, options?: RequestInit): Promise<T> {
  const res = await fetch(`${API_BASE}${path}`, {
    headers: { 'Content-Type': 'application/json' },
    ...options,
  });
  if (!res.ok) {
    const err = await res.json().catch(() => ({ message: 'Request failed' }));
    throw new Error(err.message || 'Request failed');
  }
  return res.json();
}

export const api = {
  products: {
    list: (search?: string) => request<{ data: any[] }>(`/products?search=${search || ''}`),
    get: (id: number) => request<any>(`/products/${id}`),
    create: (body: any) => request<any>(`/products`, { method: 'POST', body: JSON.stringify(body) }),
  },
  sales: {
    list: () => request<{ data: any[] }>(`/sales`),
    get: (id: number) => request<any>(`/sales/${id}`),
    create: (body: any) => request<any>(`/sales`, { method: 'POST', body: JSON.stringify(body) }),
    addPayment: (id: number, body: any) => request<any>(`/sales/${id}/payments`, { method: 'POST', body: JSON.stringify(body) }),
  },
  inventory: {
    list: () => request<any[]>(`/inventory`),
    adjust: (body: any) => request<any>(`/inventory/adjust`, { method: 'POST', body: JSON.stringify(body) }),
  },
  exchangeRates: {
    list: () => request<any[]>(`/exchange-rates`),
    convert: (amount: number, from: string, to: string) => 
      request<any>(`/exchange-rates/convert?amount=${amount}&from=${from}&to=${to}`),
  },
};

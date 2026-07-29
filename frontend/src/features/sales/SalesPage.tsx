import { useState, useEffect } from 'react'
import { api } from '../../api/client'

export default function SalesPage() {
  const [sales, setSales] = useState<any[]>([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    loadSales()
  }, [])

  const loadSales = async () => {
    setLoading(true)
    try {
      const res = await api.sales.list()
      setSales(res.data)
    } finally {
      setLoading(false)
    }
  }

  return (
    <div>
      <div className="card">
        <h2>Sales History</h2>
      </div>
      {loading ? <p>Loading...</p> : (
        <div className="card">
          <table>
            <thead>
              <tr>
                <th>Sale #</th>
                <th>Date</th>
                <th>Customer</th>
                <th>Currency</th>
                <th>Total</th>
                <th>Status</th>
                <th>Items</th>
              </tr>
            </thead>
            <tbody>
              {sales.map(s => (
                <tr key={s.saleId}>
                  <td>{s.saleNumber}</td>
                  <td>{new Date(s.saleDate).toLocaleString()}</td>
                  <td>{s.customerName || 'Walk-in'}</td>
                  <td>{s.currencyCode}</td>
                  <td>{s.total?.toFixed(2)}</td>
                  <td>{s.paymentStatus}</td>
                  <td>{s.items?.length || 0}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  )
}

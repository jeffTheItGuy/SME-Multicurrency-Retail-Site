import { useState, useEffect } from 'react'
import { api } from '../../api/client'

export default function ProductsPage() {
  const [products, setProducts] = useState<any[]>([])
  const [loading, setLoading] = useState(true)
  const [search, setSearch] = useState('')

  useEffect(() => {
    loadProducts()
  }, [search])

  const loadProducts = async () => {
    setLoading(true)
    try {
      const res = await api.products.list(search)
      setProducts(res.data)
    } finally {
      setLoading(false)
    }
  }

  return (
    <div>
      <div className="card">
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h2>Products</h2>
          <input 
            type="text" 
            placeholder="Search..." 
            value={search}
            onChange={e => setSearch(e.target.value)}
          />
        </div>
      </div>
      {loading ? <p>Loading...</p> : (
        <div className="card">
          <table>
            <thead>
              <tr>
                <th>SKU</th>
                <th>Name</th>
                <th>Category</th>
                <th>Prices</th>
                <th>Stock</th>
              </tr>
            </thead>
            <tbody>
              {products.map(p => (
                <tr key={p.productId}>
                  <td>{p.sku}</td>
                  <td>{p.productName}</td>
                  <td>{p.categoryName}</td>
                  <td>
                    {p.prices?.map((pr: any) => (
                      <div key={pr.currencyCode}>{pr.currencyCode}: {pr.price}</div>
                    ))}
                  </td>
                  <td>{p.trackInventory ? 'Yes' : 'No'}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  )
}

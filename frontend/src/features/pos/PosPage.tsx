import { useState, useEffect } from 'react'
import { api } from '../../api/client'

interface CartItem {
  productId: number
  name: string
  price: number
  quantity: number
  currency: string
}

export default function PosPage() {
  const [search, setSearch] = useState('')
  const [products, setProducts] = useState<any[]>([])
  const [cart, setCart] = useState<CartItem[]>([])
  const [currency, setCurrency] = useState('USD')
  const [paymentMethod, setPaymentMethod] = useState('Cash')
  const [reference, setReference] = useState('')
  const [loading, setLoading] = useState(false)
  const [message, setMessage] = useState('')

  useEffect(() => {
    if (search.length < 2) return
    const timer = setTimeout(() => {
      api.products.list(search).then(r => setProducts(r.data))
    }, 300)
    return () => clearTimeout(timer)
  }, [search])

  const addToCart = (product: any) => {
    const price = product.prices?.find((p: any) => p.currencyCode === currency)?.price 
      || product.prices?.[0]?.price 
      || 0
    const existing = cart.find(c => c.productId === product.productId)
    if (existing) {
      setCart(cart.map(c => c.productId === product.productId ? { ...c, quantity: c.quantity + 1 } : c))
    } else {
      setCart([...cart, { productId: product.productId, name: product.productName, price, quantity: 1, currency }])
    }
    setSearch('')
    setProducts([])
  }

  const removeFromCart = (id: number) => {
    setCart(cart.filter(c => c.productId !== id))
  }

  const total = cart.reduce((sum, c) => sum + (c.price * c.quantity), 0)

  const completeSale = async () => {
    if (cart.length === 0) return
    setLoading(true)
    try {
      const sale = await api.sales.create({
        customerId: null,
        warehouseId: 1,
        currencyCode: currency,
        items: cart.map(c => ({ productId: c.productId, quantity: c.quantity, unitPrice: c.price })),
        payments: [{
          paymentMethod: paymentMethod,
          currencyCode: currency,
          amount: total,
          referenceNumber: reference || undefined
        }]
      })
      setMessage(`Sale ${sale.saleNumber} completed!`)
      setCart([])
      setReference('')
    } catch (e: any) {
      setMessage(`Error: ${e.message}`)
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="grid grid-2">
      <div>
        <div className="card">
          <h2>Search Products</h2>
          <input
            type="text"
            placeholder="Type to search..."
            value={search}
            onChange={e => setSearch(e.target.value)}
            style={{ width: '100%', marginTop: '0.5rem' }}
          />
          {products.length > 0 && (
            <div style={{ marginTop: '0.5rem', maxHeight: '300px', overflow: 'auto' }}>
              {products.map(p => (
                <div key={p.productId} 
                  onClick={() => addToCart(p)}
                  style={{ padding: '0.5rem', cursor: 'pointer', borderBottom: '1px solid #eee' }}
                >
                  <strong>{p.productName}</strong> — {p.sku}
                  <div style={{ color: '#666', fontSize: '0.85rem' }}>
                    {p.prices?.map((pr: any) => `${pr.currencyCode}: ${pr.price}`).join(' | ')}
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>

        <div className="card">
          <h2>Payment</h2>
          <div className="grid grid-2" style={{ marginTop: '0.5rem' }}>
            <div>
              <label>Currency</label>
              <select value={currency} onChange={e => setCurrency(e.target.value)} style={{ width: '100%' }}>
                <option value="USD">USD</option>
                <option value="ZiG">ZiG</option>
              </select>
            </div>
            <div>
              <label>Method</label>
              <select value={paymentMethod} onChange={e => setPaymentMethod(e.target.value)} style={{ width: '100%' }}>
                <option value="Cash">Cash</option>
                <option value="UsdCash">USD Cash</option>
                <option value="ZigCash">ZiG Cash</option>
                <option value="EcoCash">EcoCash</option>
                <option value="Zipit">ZIPIT</option>
                <option value="BankTransfer">Bank Transfer</option>
                <option value="Card">Card</option>
              </select>
            </div>
          </div>
          <div style={{ marginTop: '0.5rem' }}>
            <label>Reference (for mobile money)</label>
            <input type="text" value={reference} onChange={e => setReference(e.target.value)} style={{ width: '100%' }} />
          </div>
        </div>
      </div>

      <div>
        <div className="card">
          <h2>Cart</h2>
          {cart.length === 0 ? (
            <p style={{ color: '#888' }}>Cart is empty</p>
          ) : (
            <table style={{ marginTop: '0.5rem' }}>
              <thead>
                <tr><th>Item</th><th>Qty</th><th>Price</th><th>Total</th><th></th></tr>
              </thead>
              <tbody>
                {cart.map(c => (
                  <tr key={c.productId}>
                    <td>{c.name}</td>
                    <td>{c.quantity}</td>
                    <td>{c.currency} {c.price.toFixed(2)}</td>
                    <td>{c.currency} {(c.price * c.quantity).toFixed(2)}</td>
                    <td>
                      <button className="btn btn-danger" onClick={() => removeFromCart(c.productId)}>×</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
          <div style={{ marginTop: '1rem', fontSize: '1.25rem', fontWeight: 600 }}>
            Total: {currency} {total.toFixed(2)}
          </div>
          <button 
            className="btn btn-success" 
            style={{ width: '100%', marginTop: '1rem', padding: '0.75rem' }}
            onClick={completeSale}
            disabled={loading || cart.length === 0}
          >
            {loading ? 'Processing...' : 'Complete Sale'}
          </button>
          {message && <p style={{ marginTop: '0.5rem', color: message.includes('Error') ? '#d32f2f' : '#388e3c' }}>{message}</p>}
        </div>
      </div>
    </div>
  )
}

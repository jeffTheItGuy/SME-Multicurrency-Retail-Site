import { Routes, Route } from 'react-router-dom'
import Layout from './components/Layout'
import PosPage from './features/pos/PosPage'
import ProductsPage from './features/products/ProductsPage'
import SalesPage from './features/sales/SalesPage'

export default function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        <Route path="/" element={<PosPage />} />
        <Route path="/products" element={<ProductsPage />} />
        <Route path="/sales" element={<SalesPage />} />
      </Route>
    </Routes>
  )
}

import { Link, Outlet } from 'react-router-dom'

export default function Layout() {
  return (
    <div>
      <nav className="nav">
        <div className="container">
          <Link to="/">POS</Link>
          <Link to="/products">Products</Link>
          <Link to="/sales">Sales</Link>
        </div>
      </nav>
      <div className="container">
        <Outlet />
      </div>
    </div>
  )
}

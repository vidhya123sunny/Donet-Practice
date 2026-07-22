import React, { useEffect, useMemo, useState } from 'react';
import { createRoot } from 'react-dom/client';
import { Boxes, RefreshCcw, Search, TrendingUp, Warehouse } from 'lucide-react';
import './styles.css';

const API_BASE = import.meta.env.VITE_API_BASE ?? 'http://localhost:5058';

async function request(path, options) {
  const response = await fetch(`${API_BASE}${path}`, {
    headers: { 'Content-Type': 'application/json' },
    ...options,
  });

  if (!response.ok) {
    const message = await response.text();
    throw new Error(message || `Request failed with status ${response.status}`);
  }

  if (response.status === 204) {
    return null;
  }

  return response.json();
}

function App() {
  const [products, setProducts] = useState([]);
  const [categories, setCategories] = useState([]);
  const [expensiveProducts, setExpensiveProducts] = useState([]);
  const [search, setSearch] = useState('');
  const [minimumPrice, setMinimumPrice] = useState('');
  const [status, setStatus] = useState('Loading inventory');
  const [isBusy, setIsBusy] = useState(false);

  const loadData = async () => {
    setIsBusy(true);
    try {
      const params = new URLSearchParams();
      if (search.trim()) params.set('search', search.trim());
      if (minimumPrice) params.set('minimumPrice', minimumPrice);

      const [productData, categoryData, reportData] = await Promise.all([
        request(`/api/products?${params}`),
        request('/api/categories'),
        request('/api/products/reports/expensive?minimumPrice=10000'),
      ]);

      setProducts(productData);
      setCategories(categoryData);
      setExpensiveProducts(reportData);
      setStatus(`Loaded ${productData.length} products`);
    } catch (error) {
      setStatus(error.message);
    } finally {
      setIsBusy(false);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  const totalStock = useMemo(
    () => products.reduce((sum, product) => sum + product.stockQuantity, 0),
    [products],
  );

  const totalValue = useMemo(
    () => products.reduce((sum, product) => sum + product.stockQuantity * product.price, 0),
    [products],
  );

  const adjustStock = async (product, quantity) => {
    setIsBusy(true);
    try {
      await request(`/api/products/${product.id}/stock`, {
        method: 'PATCH',
        body: JSON.stringify({ quantity, rowVersion: product.rowVersion }),
      });
      await loadData();
      setStatus(`Stock updated for ${product.name}`);
    } catch (error) {
      setStatus(error.message);
      setIsBusy(false);
    }
  };

  const restockAll = async () => {
    setIsBusy(true);
    try {
      const updated = await request('/api/products/batch/restock?quantity=10', { method: 'POST' });
      await loadData();
      setStatus(`Batch restocked ${updated} products`);
    } catch (error) {
      setStatus(error.message);
      setIsBusy(false);
    }
  };

  return (
    <main className="app-shell">
      <section className="toolbar">
        <div>
          <p className="eyebrow">EF Core 8.0 + ASP.NET Core Web API</p>
          <h1>Retail Inventory</h1>
        </div>
        <div className="toolbar-actions">
          <button className="icon-button" onClick={loadData} disabled={isBusy} title="Refresh inventory">
            <RefreshCcw size={18} />
          </button>
          <button className="primary-button" onClick={restockAll} disabled={isBusy}>
            <Warehouse size={18} />
            Restock +10
          </button>
        </div>
      </section>

      <section className="metrics">
        <Metric icon={<Boxes />} label="Products" value={products.length} />
        <Metric icon={<Warehouse />} label="Units in stock" value={totalStock} />
        <Metric icon={<TrendingUp />} label="Inventory value" value={currency(totalValue)} />
      </section>

      <section className="filters" aria-label="Inventory filters">
        <label>
          <Search size={17} />
          <input value={search} onChange={(event) => setSearch(event.target.value)} placeholder="Search products" />
        </label>
        <input
          className="price-filter"
          type="number"
          min="0"
          value={minimumPrice}
          onChange={(event) => setMinimumPrice(event.target.value)}
          placeholder="Minimum price"
        />
        <button onClick={loadData} disabled={isBusy}>Apply</button>
      </section>

      <section className="content-grid">
        <div className="inventory-table" role="region" aria-label="Products">
          <div className="table-row table-head">
            <span>Product</span>
            <span>Category</span>
            <span>Price</span>
            <span>Stock</span>
            <span>Actions</span>
          </div>
          {products.map((product) => (
            <div className="table-row" key={product.id}>
              <div>
                <strong>{product.name}</strong>
                <small>{product.tags.length ? product.tags.join(', ') : 'No tags'}</small>
              </div>
              <span>{product.categoryName}</span>
              <span>{currency(product.price)}</span>
              <span className={product.stockQuantity < 20 ? 'low-stock' : ''}>{product.stockQuantity}</span>
              <div className="stock-actions">
                <button onClick={() => adjustStock(product, -1)} disabled={isBusy || product.stockQuantity === 0}>-</button>
                <button onClick={() => adjustStock(product, 1)} disabled={isBusy}>+</button>
              </div>
            </div>
          ))}
        </div>

        <aside className="side-panel">
          <h2>Categories</h2>
          {categories.map((category) => (
            <div className="side-row" key={category.id}>
              <span>{category.name}</span>
              <strong>{category.productCount}</strong>
            </div>
          ))}

          <h2>Compiled query report</h2>
          {expensiveProducts.map((product) => (
            <div className="report-row" key={product.name}>
              <span>{product.name}</span>
              <strong>{currency(product.price)}</strong>
            </div>
          ))}
        </aside>
      </section>

      <footer>{status}</footer>
    </main>
  );
}

function Metric({ icon, label, value }) {
  return (
    <div className="metric">
      {React.cloneElement(icon, { size: 20 })}
      <span>{label}</span>
      <strong>{value}</strong>
    </div>
  );
}

function currency(value) {
  return new Intl.NumberFormat('en-IN', {
    style: 'currency',
    currency: 'INR',
    maximumFractionDigits: 0,
  }).format(value);
}

createRoot(document.getElementById('root')).render(<App />);

import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { Provider } from 'react-redux';
import { store } from './redux/store';
import Home from './pages/Home';
import SalesOrder from './pages/SalesOrder';

function App() {
  return (
    <Provider store={store}>
      <Router>
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/sales-order" element={<SalesOrder />} />
          <Route path="/sales-order/:id" element={<SalesOrder />} />
        </Routes>
      </Router>
    </Provider>
  );
}

export default App;
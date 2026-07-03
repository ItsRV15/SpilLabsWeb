import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { Link } from 'react-router-dom';
import { fetchOrders } from '../redux/slices/ordersSlice';
import { Table } from '../components/Table';

const Home = () => {
  const dispatch = useDispatch();
  const { orders, loading } = useSelector((state) => state.orders);

  useEffect(() => {
    dispatch(fetchOrders());
  }, [dispatch]);

  const columns = [
    {
      key: 'id',
      title: 'Order ID',
    },
    {
      key: 'customerName',
      title: 'Customer Name',
    },
    {
      key: 'invoiceNo',
      title: 'Invoice No',
    },
    {
      key: 'invoiceDate',
      title: 'Invoice Date',
      render: (value) => value ? new Date(value).toLocaleDateString() : '-',
    },
    {
      key: 'totalIncl',
      title: 'Total Amount',
      render: (value) => value ? `$${parseFloat(value).toFixed(2)}` : '$0.00',
    },
    {
      key: 'createdAt',
      title: 'Created Date',
      render: (value) => value ? new Date(value).toLocaleDateString() : '-',
    },
  ];

  const handleRowClick = (order) => {
    // Navigate to edit order
    window.location.href = `/sales-order/${order.id}`;
  };

  return (
    <div className="min-h-screen bg-gray-100">
      {/* Header */}
      <div className="bg-white shadow-sm border-b border-gray-300">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
          <div className="flex justify-between items-center">
            <h1 className="text-2xl font-bold text-gray-900">Home</h1>
            <Link to="/sales-order">
              <button className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 transition-colors duration-200">
                Add New
              </button>
            </Link>
          </div>
        </div>
      </div>

      {/* Content */}
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
        <div className="bg-white border border-gray-300 rounded-sm">
          <div className="overflow-x-auto">
            <Table
              columns={columns}
              data={orders}
              onRowClick={handleRowClick}
              loading={loading}
            />
          </div>
        </div>
      </div>
    </div>
  );
};

export default Home;
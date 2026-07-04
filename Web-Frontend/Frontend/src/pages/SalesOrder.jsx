import React, { useEffect, useState, useMemo } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { useNavigate, useParams } from 'react-router-dom';
import { fetchCustomers } from '../redux/slices/customersSlice';
import { fetchItems } from '../redux/slices/itemsSlice';
import { createOrder, updateOrder, fetchOrders } from '../redux/slices/ordersSlice';
import { Input, Select, TextArea, Button } from '../components/FormControls';

const SalesOrder = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const dispatch = useDispatch();
  const { customers } = useSelector((state) => state.customers);
  const { items } = useSelector((state) => state.items);
  const { orders } = useSelector((state) => state.orders);

  const [formData, setFormData] = useState({
    customerId: '',
    customerName: '',
    address1: '',
    address2: '',
    address3: '',
    suburb: '',
    state: '',
    postCode: '',
    invoiceNo: '',
    invoiceDate: new Date().toISOString().split('T')[0],
    referenceNo: '',
    note: '',
    items: [],
    totalExcl: 0,
    totalTax: 0,
    totalIncl: 0,
  });

  const initialInvoiceNo = useMemo(() => '', []);
  const initialReferenceNo = useMemo(() => '', []);

  const [loading, setLoading] = useState(false);

  // Fetch customers and items on mount
  useEffect(() => {
    dispatch(fetchCustomers());
    dispatch(fetchItems());
  }, [dispatch]);

  // Load existing order if editing
  useEffect(() => {
    if (id) {
      // If orders not loaded yet, fetch them
      if (orders.length === 0) {
        dispatch(fetchOrders()).then(() => {
          const order = orders.find(o => o.id === parseInt(id));
          if (order) {
            setFormData(order);
          }
        });
      } else {
        const order = orders.find(o => o.id === parseInt(id));
        if (order) {
          setFormData(order);
        }
      }
    }
  }, [id, dispatch, orders]);

  // Auto-fill address when customer is selected
  useEffect(() => {
    if (formData.customerId) {
      const selectedCustomer = customers.find(c => c.id === parseInt(formData.customerId));
      if (selectedCustomer) {
        setFormData(prev => ({
          ...prev,
          customerName: selectedCustomer.name,
          address1: selectedCustomer.address1 || '',
          address2: selectedCustomer.address2 || '',
          address3: selectedCustomer.address3 || '',
          suburb: selectedCustomer.suburb || '',
          state: selectedCustomer.state || '',
          postCode: selectedCustomer.postCode || '',
        }));
      }
    }
  }, [formData.customerId, customers]);

  // Calculate totals whenever items change
  useEffect(() => {
    const totals = formData.items.reduce(
      (acc, item) => {
        acc.totalExcl += parseFloat(item.exclAmount) || 0;
        acc.totalTax += parseFloat(item.taxAmount) || 0;
        acc.totalIncl += parseFloat(item.inclAmount) || 0;
        return acc;
      },
      { totalExcl: 0, totalTax: 0, totalIncl: 0 }
    );

    setFormData(prev => ({
      ...prev,
      ...totals,
    }));
  }, [formData.items]);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const normalizeOrderPayload = (data) => ({
    ...data,
    invoiceNo: data.invoiceNo?.trim() || initialInvoiceNo,
    referenceNo: data.referenceNo?.trim() || initialReferenceNo,
    customerId: parseInt(data.customerId, 10),
    totalExcl: parseFloat(data.totalExcl),
    totalTax: parseFloat(data.totalTax),
    totalIncl: parseFloat(data.totalIncl),
    items: data.items.map(item => ({
      ...item,
      quantity: parseFloat(item.quantity),
      price: parseFloat(item.price),
      taxRate: parseFloat(item.taxRate),
      exclAmount: parseFloat(item.exclAmount),
      taxAmount: parseFloat(item.taxAmount),
      inclAmount: parseFloat(item.inclAmount),
    })),
  });

  const handleCustomerChange = (e) => {
    const value = e.target.value;
    setFormData(prev => ({ ...prev, customerId: value }));
  };

  const handleItemChange = (index, field, value) => {
    const updatedItems = [...formData.items];
    updatedItems[index][field] = value;

    // If item code or description changed, update price
    if (field === 'itemCode' || field === 'description') {
      const selectedItem = items.find(
        item => item.code === value || item.description === value
      );
      if (selectedItem) {
        updatedItems[index].price = selectedItem.price;
        updatedItems[index].itemCode = selectedItem.code;
        updatedItems[index].description = selectedItem.description;
      }
    }

    // Recalculate amounts if quantity, price, or tax rate changed
    if (['quantity', 'price', 'taxRate'].includes(field)) {
      const item = updatedItems[index];
      const quantity = parseFloat(item.quantity) || 0;
      const price = parseFloat(item.price) || 0;
      const taxRate = parseFloat(item.taxRate) || 0;

      const exclAmount = quantity * price;
      const taxAmount = exclAmount * taxRate / 100;
      const inclAmount = exclAmount + taxAmount;

      updatedItems[index].exclAmount = exclAmount.toFixed(2);
      updatedItems[index].taxAmount = taxAmount.toFixed(2);
      updatedItems[index].inclAmount = inclAmount.toFixed(2);
    }

    setFormData(prev => ({ ...prev, items: updatedItems }));
  };

  const addItem = () => {
    setFormData(prev => ({
      ...prev,
      items: [...prev.items, {
        itemCode: '',
        description: '',
        note: '',
        quantity: 0,
        price: 0,
        taxRate: 0,
        exclAmount: 0,
        taxAmount: 0,
        inclAmount: 0,
      }],
    }));
  };

  const removeItem = (index) => {
    const updatedItems = formData.items.filter((_, i) => i !== index);
    setFormData(prev => ({ ...prev, items: updatedItems }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);

    try {
      const orderData = normalizeOrderPayload(formData);

      if (id) {
        await dispatch(updateOrder({ id: parseInt(id), orderData })).unwrap();
      } else {
        await dispatch(createOrder(orderData)).unwrap();
      }

      await dispatch(fetchOrders());
      navigate('/');
    } catch (error) {
      console.error('Error saving order:', error);
      alert('Error saving order. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  const handlePrint = () => {
    window.print();
  };

  // Prepare options for dropdowns
  const customerOptions = customers.map(customer => ({
    value: customer.id,
    label: customer.name,
  }));

  const itemCodeOptions = items.map(item => ({
    value: item.code,
    label: item.code,
  }));

  const descriptionOptions = items.map(item => ({
    value: item.description,
    label: item.description,
  }));

  return (
    <div className="min-h-screen bg-gray-100">
      {/* Header */}
      <div className="bg-white shadow-sm border-b border-gray-300">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
          <div className="flex justify-between items-center">
            <h1 className="text-2xl font-bold text-gray-900">Sales Order</h1>
            <div className="flex gap-2">
              <Button variant="secondary" onClick={() => navigate('/')}>
                Cancel
              </Button>
              <Button variant="success" onClick={handlePrint} type="button">
                Print
              </Button>
              <Button variant="primary" onClick={handleSubmit} loading={loading} type="submit">
                Save Order
              </Button>
            </div>
          </div>
        </div>
      </div>

      {/* Form */}
      <form onSubmit={handleSubmit}>
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
          <div className="bg-white border border-gray-300 rounded-sm p-6 mb-6">
            {/* Customer and Invoice Details */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-6">
              <Select
                label="Customer Name"
                name="customerId"
                value={formData.customerId}
                onChange={handleCustomerChange}
                options={customerOptions}
                placeholder="Select Customer"
                required
              />
              <Input
                label="Invoice No."
                name="invoiceNo"
                //value={formData.invoiceNo}
                //onChange={handleInputChange}
                placeholder="Enter invoice number"
                required
              />
              <Input
                label="Invoice Date"
                name="invoiceDate"
                type="date"
                value={formData.invoiceDate}
                onChange={handleInputChange}
                required
              />
              <Input
                label="Reference No"
                name="referenceNo"
                //value={formData.referenceNo}
                //onChange={handleInputChange}
                placeholder="Enter reference number"
              />
            </div>

            {/* Address Fields */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-6">
              <Input
                label="Address 1"
                name="address1"
                value={formData.address1}
                onChange={handleInputChange}
                placeholder="Address line 1"
              />
              <Input
                label="Address 2"
                name="address2"
                value={formData.address2}
                onChange={handleInputChange}
                placeholder="Address line 2"
              />
              <Input
                label="Address 3"
                name="address3"
                value={formData.address3}
                onChange={handleInputChange}
                placeholder="Address line 3"
              />
              <Input
                label="Suburb"
                name="suburb"
                value={formData.suburb}
                onChange={handleInputChange}
                placeholder="Suburb"
              />
              <Input
                label="State"
                name="state"
                value={formData.state}
                onChange={handleInputChange}
                placeholder="State"
              />
              <Input
                label="Post Code"
                name="postCode"
                value={formData.postCode}
                onChange={handleInputChange}
                placeholder="Post code"
              />
            </div>

            {/* Note */}
            <TextArea
              label="Note"
              name="note"
              value={formData.note}
              onChange={handleInputChange}
              placeholder="Enter order notes"
              rows={3}
            />
          </div>

          {/* Line Items */}
          <div className="bg-white border border-gray-300 rounded-sm p-6 mb-6">
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-lg font-semibold text-gray-900">Order Items</h2>
              <Button type="button" variant="secondary" onClick={addItem}>
                Add Item
              </Button>
            </div>

            <div className="overflow-x-auto">
              <table className="min-w-full border border-gray-300">
                <thead className="bg-gray-100">
                  <tr>
                    <th className="px-3 py-2 text-left text-sm font-semibold text-gray-700 border-b border-gray-300">Item Code</th>
                    <th className="px-3 py-2 text-left text-sm font-semibold text-gray-700 border-b border-gray-300">Description</th>
                    <th className="px-3 py-2 text-left text-sm font-semibold text-gray-700 border-b border-gray-300">Note</th>
                    <th className="px-3 py-2 text-left text-sm font-semibold text-gray-700 border-b border-gray-300">Quantity</th>
                    <th className="px-3 py-2 text-left text-sm font-semibold text-gray-700 border-b border-gray-300">Price</th>
                    <th className="px-3 py-2 text-left text-sm font-semibold text-gray-700 border-b border-gray-300">Tax</th>
                    <th className="px-3 py-2 text-left text-sm font-semibold text-gray-700 border-b border-gray-300">Excl Amount</th>
                    <th className="px-3 py-2 text-left text-sm font-semibold text-gray-700 border-b border-gray-300">Tax Amount</th>
                    <th className="px-3 py-2 text-left text-sm font-semibold text-gray-700 border-b border-gray-300">Incl Amount</th>
                    <th className="px-3 py-2 text-left text-sm font-semibold text-gray-700 border-b border-gray-300">Action</th>
                  </tr>
                </thead>
                <tbody>
                  {formData.items.map((item, index) => (
                    <tr key={index} className={index % 2 === 0 ? 'bg-white' : 'bg-gray-50'}>
                      <td className="px-3 py-2 border-b border-gray-200">
                        <Select
                          value={item.itemCode}
                          onChange={(e) => handleItemChange(index, 'itemCode', e.target.value)}
                          options={itemCodeOptions}
                          placeholder="Select"
                          className="mb-0"
                        />
                      </td>
                      <td className="px-3 py-2 border-b border-gray-200">
                        <Select
                          value={item.description}
                          onChange={(e) => handleItemChange(index, 'description', e.target.value)}
                          options={descriptionOptions}
                          placeholder="Select"
                          className="mb-0"
                        />
                      </td>
                      <td className="px-3 py-2 border-b border-gray-200">
                        <Input
                          value={item.note}
                          onChange={(e) => handleItemChange(index, 'note', e.target.value)}
                          placeholder="Note"
                          className="mb-0"
                        />
                      </td>
                      <td className="px-3 py-2 border-b border-gray-200">
                        <Input
                          type="number"
                          value={item.quantity}
                          onChange={(e) => handleItemChange(index, 'quantity', e.target.value)}
                          placeholder="0"
                          min="0"
                          step="0.01"
                          className="mb-0"
                        />
                      </td>
                      <td className="px-3 py-2 border-b border-gray-200">
                        <Input
                          type="number"
                          value={item.price}
                          onChange={(e) => handleItemChange(index, 'price', e.target.value)}
                          placeholder="0.00"
                          min="0"
                          step="0.01"
                          className="mb-0"
                        />
                      </td>
                      <td className="px-3 py-2 border-b border-gray-200">
                        <Input
                          type="number"
                          value={item.taxRate}
                          onChange={(e) => handleItemChange(index, 'taxRate', e.target.value)}
                          placeholder="0"
                          min="0"
                          step="0.01"
                          className="mb-0"
                        />
                      </td>
                      <td className="px-3 py-2 border-b border-gray-200 text-sm text-gray-700">
                        ${parseFloat(item.exclAmount).toFixed(2)}
                      </td>
                      <td className="px-3 py-2 border-b border-gray-200 text-sm text-gray-700">
                        ${parseFloat(item.taxAmount).toFixed(2)}
                      </td>
                      <td className="px-3 py-2 border-b border-gray-200 text-sm text-gray-700">
                        ${parseFloat(item.inclAmount).toFixed(2)}
                      </td>
                      <td className="px-3 py-2 border-b border-gray-200">
                        <Button
                          type="button"
                          variant="danger"
                          onClick={() => removeItem(index)}
                          className="px-2 py-1 text-sm"
                        >
                          Remove
                        </Button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>

            {formData.items.length === 0 && (
              <div className="text-center py-8 text-gray-500">
                No items added. Click "Add Item" to add items to the order.
              </div>
            )}
          </div>

          {/* Totals */}
          <div className="bg-white border border-gray-300 rounded-sm p-6">
            <div className="flex justify-end">
              <div className="w-80 space-y-3">
                <div className="flex justify-between items-center">
                  <span className="text-sm font-medium text-gray-700">Total Excl:</span>
                  <Input
                    type="number"
                    value={formData.totalExcl.toFixed(2)}
                    readOnly
                    className="w-40 mb-0 text-right"
                  />
                </div>
                <div className="flex justify-between items-center">
                  <span className="text-sm font-medium text-gray-700">Total Tax:</span>
                  <Input
                    type="number"
                    value={formData.totalTax.toFixed(2)}
                    readOnly
                    className="w-40 mb-0 text-right"
                  />
                </div>
                <div className="flex justify-between items-center">
                  <span className="text-base font-semibold text-gray-900">Total Incl:</span>
                  <Input
                    type="number"
                    value={formData.totalIncl.toFixed(2)}
                    readOnly
                    className="w-40 mb-0 text-right font-semibold"
                  />
                </div>
              </div>
            </div>
          </div>
        </div>
      </form>
    </div>
  );
};

export default SalesOrder;
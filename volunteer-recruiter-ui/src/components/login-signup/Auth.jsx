import React, { useState } from 'react';
import './Auth.css';
import InputField from './InputField';

const Auth = () => {

  const [isSignup, setIsSignup] = useState(false);
  const [formData, setFormData] = useState({
    firstName: '',
    surname: '',
    email: '',
    password: ''
  });

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  const handleFormSwitch = (flag) => {
    setIsSignup(flag);
    setFormData({
      firstName: '',
      surname: '',
      email: '',
      password: ''
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    const dataToSend = { ...formData, operation: isSignup ? 'signup' : 'login' };

    console.log('Form Data:', dataToSend); // Log form data to the console

    const url = isSignup ? 'http://localhost:5000/api/signup' : 'http://localhost:5000/api/login'; // Update with backend URL
    try {
      const response = await fetch(url, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(formData)
      });
      const result = await response.json();
      if (response.ok) {
        alert('Success: ' + JSON.stringify(result));
      } else {
        alert('Error: ' + result.message);
      }
    } catch (error) {
      console.error('Error:', error);
      alert('Error: ' + error.message);
    }
  };

  return (
    <div>
      <div>
        <button onClick={() => handleFormSwitch(false)}>Login</button>
        <button onClick={() => handleFormSwitch(true)}>Signup</button>
      </div>
      {isSignup ? (
        <div>
          <h2>Signup</h2>
          <form onSubmit={handleSubmit}>
          <div className="form-group">
          <InputField 
              label="First Name:" 
              type="text" 
              name="firstName" 
              value={formData.firstName} 
              onChange={handleInputChange} 
              required 
            />
          </div>
            
            <div className="form-group">
            <InputField 
              label="Surname:" 
              type="text" 
              name="surname" 
              value={formData.surname} 
              onChange={handleInputChange} 
              required 
            />
            </div>
            
            <div className="form-group">
            <InputField 
              label="Email:" 
              type="email" 
              name="email" 
              value={formData.email} 
              onChange={handleInputChange} 
              required 
            />
            </div>
            
            <div className="form-group">
            <InputField 
              label="Password:" 
              type="password" 
              name="password" 
              value={formData.password} 
              onChange={handleInputChange} 
              required 
            />
            </div>
            
            <button type="submit">Register</button>
          </form>
        </div>
      ) : (
        <div>
          <h2>Login</h2>
          <form onSubmit={handleSubmit}>
          <div className="form-group">
          <InputField 
              label="Email:" 
              type="email" 
              name="email" 
              value={formData.email} 
              onChange={handleInputChange} 
              required 
            />
          </div>
            
          <div className="form-group">
          <InputField 
              label="Password:" 
              type="password" 
              name="password" 
              value={formData.password} 
              onChange={handleInputChange} 
              required 
            />
          </div>
            
            <button type="submit">Login</button>
          </form>
        </div>
      )}
    </div>
  );
};

export default Auth;

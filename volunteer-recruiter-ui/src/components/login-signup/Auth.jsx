import React, { useState } from 'react';
import './Auth.css';
import InputField from './InputField';

const Auth = () => {

  const [isSignup, setIsSignup] = useState(false);
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    phone: ''
  });

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  const handleFormSwitch = (flag) => {
    setIsSignup(flag);
    setFormData({
      firstName: '',
      lastName: '',
      email: '',
      password: '',
      phone: ''
    });
  };
  const handleLogout = async () => {
    try {
      const response = await fetch( 'http://localhost:9000/logout', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ connectionKey: document.cookie.split('=')[1] })
      });

      console.log(JSON.stringify({ connectionKey: document.cookie.split('=')[1] })); // Log form data to the console
      const result = await response.json();
      handleResponse(result);
    } catch (error) {
      console.error('Error:', error);
      alert('Error222: ' + error.message);
    }
    document.cookie = "connectionKey=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
    setIsSignup(false);
    setFormData({
      firstName: '',
      lastName: '',
      email: '',
      password: '',
      phone: ''
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    const dataToSend = { ...formData };

    console.log('Form Data:', dataToSend); // Log form data to the console

    const url = isSignup ? 'http://localhost:9000/signup' : 'http://localhost:9000/login';
    console.log('URL:', url); // Log URL to the console
    try {
      const response = await fetch(url, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(formData)
      });
      const result = await response.json();
      handleResponse(result);
    } catch (error) {
      console.error('Error:', error);
      alert('Error222: ' + error.message);
    }
  };
  const handleResponse = (response) => {
      console.log(response);
      try {
        const operationStatus = JSON.parse(response.operationStatus);
        if (operationStatus.Code === 0) {
          alert(operationStatus.Message);
          document.cookie = `connectionKey=${response.connectionKey}; path=/;`;
        }else if(operationStatus.Code === 3 || operationStatus.Code === 2){
          alert('Error: ' + operationStatus.Message);
        } else {
          alert('Failed: ' + operationStatus.Message);
        }
      } catch (error) {
        console.error('Error parsing response:', error);
        alert('Error: Invalid response format');
      }
  };

  return (
    <div>
      <div>
        <button onClick={() => handleFormSwitch(false)}>Log In</button>
        <button onClick={() => handleFormSwitch(true)}>Sign Up</button>
        <button onClick={() => handleLogout()}>Log Out</button>
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
              label="Last Name:" 
              type="text" 
              name="lastName" 
              value={formData.lastName} 
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

            <div className="form-group">
            <InputField 
              label="Phone:" 
              type="phone" 
              name="phone" 
              value={formData.phone} 
              onChange={handleInputChange} 
              required 
            />
            </div>
            
            <button type="submit">Submit</button>
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
            
            <button type="submit">Submit</button>
          </form>
        </div>
      )}
    </div>
  );
};

export default Auth;

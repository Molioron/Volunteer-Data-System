import React from 'react';
import Button from '../common/Button';
import InputField from '../common/InputField';

/**
 * `AuthInputField` renders input fields for login or signup forms based on the `isSignup` prop.
 * 
 * Props:
 * - `isSignup` (boolean): Determines if the form is for signup (true) or login (false).
 * - `formData` (object): Contains form data for each field (firstName, lastName, email, etc.).
 * - `handleInputChange` (function): Updates form data on input change.
 * - `handleSubmit` (function): Handles form submission.
 * 
 * Usage:
 * ```jsx
 * <AuthInputField
 *    isSignup={isSignup}
 *    formData={formData}
 *    handleInputChange={handleInputChange}
 *    handleSubmit={handleSubmit}
 * />
 * ```
 */

const AuthInputField = ({ isSignup, formData, handleInputChange, handleSubmit }) => {
  return (
     // Renders the appropriate fields for login or signup based on `isSignup`
    <form onSubmit={handleSubmit}> 
      {isSignup ? ( 
        <>
          <h2>Signup</h2>
          <InputField label="First Name:" type="text" name="firstName" value={formData.firstName} onChange={handleInputChange} required autoComplete="given-name" />
          <InputField label="Last Name:" type="text" name="lastName" value={formData.lastName} onChange={handleInputChange} required autoComplete="family-name" />
          <InputField label="Email:" type="email" name="email" value={formData.email} onChange={handleInputChange} required autoComplete="email" />
          <InputField label="Password:" type="password" name="password" value={formData.password} onChange={handleInputChange} required autoComplete="new-password" />
          <InputField label="Phone:" type="tel" name="phone" value={formData.phone} onChange={handleInputChange} required autoComplete="tel" />
        </>
      ) : ( // If isSignup is false, display Login form fields
        <>
          <h2>Login</h2>
          <InputField label="Email:" type="email" name="email" value={formData.email} onChange={handleInputChange} required autoComplete="email" />
          <InputField label="Password:" type="password" name="password" value={formData.password} onChange={handleInputChange} required autoComplete="current-password" />
        </>
      )}
      <div className="auth-action-button">
        <Button type="submit">Submit</Button>
      </div>
    </form>
  );
};

export default AuthInputField;

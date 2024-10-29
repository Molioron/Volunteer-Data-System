import React from 'react';

/**
 * `InputField` component provides a flexible input with support for various types,
 * including text, select, and multiline textarea.
 * 
 * Props:
 * - `label` (string): Label text displayed above the input.
 * - `type` (string): Type of input ('text', 'password', 'select', etc.).
 * - `name` (string): Name attribute of the input element.
 * - `value` (string): Value of the input field.
 * - `onChange` (function): Callback function triggered on input value change.
 * - `required` (boolean, optional): Marks the input as required if true.
 * - `autoComplete` (string, optional): Autocomplete setting for the input field.
 * - `options` (array, optional): List of options for select inputs, each option should be an object with `value` and `label`.
 * - `multiline` (boolean, optional): Renders a textarea if true, otherwise a single-line input.
 * - `rows` (number, optional): Number of rows for the textarea.
 * - `className` (string, optional): Additional CSS classes for custom styling.
 * 
 * Usage:
 * ```jsx
 * <InputField 
 *    label="Name" 
 *    type="text" 
 *    name="username" 
 *    value={username} 
 *    onChange={handleInputChange} 
 *    required 
 * />
 * 
 * <InputField 
 *    label="Description" 
 *    multiline 
 *    rows={4} 
 *    name="description" 
 *    value={description} 
 *    onChange={handleInputChange} 
 * />
 * ```
 */

const InputField = ({ label, type, name, value, onChange, required, autoComplete, options, multiline, rows, className }) => {
  return (
    <div className={className || "form-group"}>
      <label>{label}</label>
      {multiline ? (
        <textarea
          name={name}
          value={value}
          onChange={onChange}
          required={required}
          rows={rows}
        />
      ) : type === "select" ? (
        <select name={name} value={value} onChange={onChange}>
          {options.map((option) => (
            <option key={option.value} value={option.value}>
              {option.label}
            </option>
          ))}
        </select>
      ) : (
        <input
          type={type}
          name={name}
          value={value}
          onChange={onChange}
          required={required}
          autoComplete={autoComplete}
        />
      )}
    </div>
  );
};

export default InputField;
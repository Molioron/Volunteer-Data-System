import React from 'react';

// A generic reusable input field component
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
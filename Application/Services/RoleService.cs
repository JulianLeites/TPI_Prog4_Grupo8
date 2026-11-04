using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private static readonly HashSet<int> ProtectedRoleIds = new()
    {
        1, // admin
        2, // waiter
        3, // kitchen
        4 // user
    };

    public RoleService(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<IEnumerable<RoleDto>> GetAllAsync()
    {
        var roles = await _roleRepository.GetAllAsync();

        return roles.Select(role => new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
        });

    }

    public async Task<RoleDto?> GetByIdAsync(int id)
    {
        var role = await _roleRepository.GetByIdAsync(id);

        if (role is null)
            return null;

        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
        };
    }

    public async Task<RoleDto?> UpdateAsync(int id, RoleUpdateDto dto)
    {
        var role = await _roleRepository.GetByIdAsync(id);

        if (role is null)
            return null;

        if (ProtectedRoleIds.Contains(id))
            throw new InvalidOperationException("No se pueden modificar los roles predefinidos del sistema");

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("El nombre del rol es obligatorio");

        var name = dto.Name.Trim().ToLowerInvariant();

        var exists = await _roleRepository.ExistsByNameAsync(name, id);

        if (exists)
            throw new InvalidOperationException("Ya existe un rol con ese nombre");

        role.Name = name;

        await _roleRepository.UpdateAsync(role);

        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var role = await _roleRepository.GetByIdAsync(id);

        if (role is null)
            return false;

        if (ProtectedRoleIds.Contains(id))
            throw new InvalidOperationException("No se puede eliminar un rol predefinido del sistema");

        var hasUsers = await _roleRepository.HasUsersAsync(id);

        if (hasUsers)
            throw new InvalidOperationException("No se puede eliminar un rol con usuarios asociados");

        await _roleRepository.DeleteAsync(role);
        return true;
    }

    public async Task<RoleDto> CreateAsync(RoleCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("El nombre del rol es obligatorio");
        
        var name = dto.Name.Trim().ToLowerInvariant();

        var exists = await _roleRepository.ExistsByNameAsync(name);
        if (exists)
            throw new InvalidOperationException("Ya existe un rol con ese nombre");

        var role = new Role
        {
            Name = name
        };

        await _roleRepository.AddAsync(role);

        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
        };
    }
}
